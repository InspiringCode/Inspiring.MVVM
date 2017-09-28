namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   /// <summary>
   ///   Notes: Currently not used. Does not work correctly if item is contained mulitple 
   ///   collections/child properties (see tests). If This really saves some performance,
   ///   one solution to fix this behavior is to change 'RemoveCachedResult' with the
   ///   following pseudo code:
   ///      if (oldItem.Parents.CountOf(self) == 1) {
   ///         RemoveCache();
   ///      } else {
   ///         // Ignore, because we still reference it.
   ///      }
   /// </summary>
   internal sealed class CachingValidationResultAggregatorBehavior :
      Behavior,
      IBehaviorInitializationBehavior,
      IValidationResultAggregatorBehavior,
      IChangeHandlerBehavior {

      private DynamicFieldAccessor<ResultCacheManager> _cache;

      public void Initialize(BehaviorInitializationContext context) {
         _cache = new DynamicFieldAccessor<ResultCacheManager>(context, ViewModel.GeneralFieldGroup);
         this.InitializeNext(context);
      }

      public ValidationResult GetValidationResult(IBehaviorContext context, ValidationResultScope scope) {
         var c = GetCache(context);
         return c.GetResult(scope);
      }

      public void HandleChange(IBehaviorContext context, ChangeArgs args) {
         var c = GetCache(context);
         c.HandleChange(context, args);

         this.HandleChangedNext(context, args);
      }

      private ResultCacheManager GetCache(IBehaviorContext context) {
         ResultCacheManager c;
         if (!_cache.TryGet(context, out c)) {
            c = new ResultCacheManager();
            _cache.Set(context, c);
         }

         return c;
      }


      private class ResultCacheManager {
         private readonly ChildResultCollection _childResults = new ChildResultCollection();

         private ValidationResult _propertiesResult = ValidationResult.Valid;
         private ValidationResult _viewModelResult = ValidationResult.Valid;
         private ValidationResult _selfResult = ValidationResult.Valid;
         private ValidationResult _allResult = ValidationResult.Valid;

         public ValidationResult GetResult(ValidationResultScope scope) {
            switch (scope) {
               case ValidationResultScope.All:
                  return _allResult;
               case ValidationResultScope.Self:
                  return _selfResult;
               case ValidationResultScope.Descendants:
                  return _childResults.JoinedResult;
               case ValidationResultScope.ViewModelValidationsOnly:
                  return _viewModelResult;
               case ValidationResultScope.PropertiesOnly:
                  return _propertiesResult;
               default:
                  throw new NotSupportedException();
            }
         }

         // We eagerly refresh the caches of all ancestors. Consider the case:
         //   (1) A validation result change is notified, thus all caches up the
         //       hierarchy hold invalid validation states.
         //   (2) The next behavior in this chain may access the cached validation
         //       result of an ancestor (this is actually the case with the 
         //       'CollectionValidationController').
         // 
         // Note that we do not handle changes that originated from an descendant
         // because we have already handled them (the caches of all ancestors were
         // cleared when the change was handled on that descendant).
         public void HandleChange(IBehaviorContext context, ChangeArgs args) {
            bool refreshDescendants = false;
            bool resultsChanged = false;
            var self = context.VM;

            switch (args.ChangeType) {
               case ChangeType.ValidationResultChanged:
                  var ownViewModelResultChanged = args
                     .ChangedPath
                     .SelectsOnly(self)
                     .Success;

                  if (ownViewModelResultChanged) {
                     RefreshViewModelResult(context);
                     resultsChanged = true;
                  } else {
                     var ownPropertyResultChanged = args
                        .ChangedPath
                        .SelectsOnlyPropertyOf(self)
                        .Success;

                     if (ownPropertyResultChanged) {
                        RefreshPropertiesResult(context);
                        resultsChanged = true;
                     }
                  }
                  break;

               case ChangeType.PropertyChanged:
                  var ownViewModelPropertyMayHaveChanged = args
                     .ChangedPath
                     .SelectsOnlyPropertyOf(self)
                     .Success;

                  refreshDescendants = ownViewModelPropertyMayHaveChanged;
                  break;

               case ChangeType.AddedToCollection:
               case ChangeType.RemovedFromCollection:
                  var ownCollectionContentsChanged = args
                     .ChangedPath
                     .SelectsOnlyCollectionOf(self)
                     .Success;

                  refreshDescendants = ownCollectionContentsChanged;
                  break;
            }

            if (refreshDescendants) {
               RefreshChildren(
                  context,
                  removedChildren: args.OldItems,
                  newChildren: args.NewItems
               );
            } else if (resultsChanged) {
               RefreshDerivedResults();
               NotifyBehaviorOfParent(context);
            }
         }

         private void RefreshChildren(
            IBehaviorContext context,
            IEnumerable<IViewModel> removedChildren = null,
            IEnumerable<IViewModel> newChildren = null
         ) {
            bool descendantsResultChanged = false;

            if (removedChildren != null) {
               foreach (var c in removedChildren) {
                  if (_childResults.RemoveCachedResult(c)) {
                     descendantsResultChanged = true;
                  }
               }
            }

            if (newChildren != null) {
               foreach (var c in newChildren) {
                  if (_childResults.UpdateCachedResult(c)) {
                     descendantsResultChanged = true;
                  }
               }
            }

            if (descendantsResultChanged) {
               RefreshDerivedResults();
               NotifyBehaviorOfParent(context);
            }
         }

         private void RefreshPropertiesResult(IBehaviorContext context) {
            var props = context
               .VM
               .Descriptor
               .Properties;

            var results = props.Select(x => x
               .Behaviors
               .GetValidationResultNext(context)
            );

            _propertiesResult = ValidationResult.Join(results);
         }

         private void RefreshViewModelResult(IBehaviorContext context) {
            _viewModelResult = context
               .VM
               .Descriptor
               .Behaviors
               .GetValidationResultNext(context);
         }

         private void RefreshDerivedResults() {
            _selfResult = ValidationResult.Join(_propertiesResult, _viewModelResult);
            _allResult = ValidationResult.Join(_selfResult, _childResults.JoinedResult);
         }

         private void NotifyBehaviorOfParent(IBehaviorContext context) {
            foreach (IViewModel parent in context.VM.Kernel.Parents) {
               CachingValidationResultAggregatorBehavior parentBehavior;
               bool parentHasBehavior = parent
                  .Descriptor
                  .Behaviors
                  .TryGetBehavior<CachingValidationResultAggregatorBehavior>(out parentBehavior);

               if (parentHasBehavior) {
                  var parentContext = parent.GetContext();
                  var parentCacheManager = parentBehavior.GetCache(parentContext);

                  parentCacheManager.RefreshChildren(
                     parentContext,
                     newChildren: new[] { context.VM }
                  );
               }
            }
         }
      }

      /// <summary>
      ///   A cache that holds all invalid direct children of the corrent VM and
      ///   manages them performantly.
      /// </summary>
      private class ChildResultCollection {
         private readonly List<ChildResultEntry> _entries = new List<ChildResultEntry>();

         public ChildResultCollection() {
            JoinedResult = ValidationResult.Valid;
         }

         public ValidationResult JoinedResult {
            get;
            private set;
         }

         public bool RemoveCachedResult(IViewModel child) {
            Check.NotNull(child, nameof(child));

            int removals = _entries.RemoveAll(x => x.Child == child);
            bool joinedResultHasChanged = removals >= 1;

            if (joinedResultHasChanged) {
               RefreshJoinedResult();
            }

            return joinedResultHasChanged;
         }

         public bool UpdateCachedResult(IViewModel child) {
            Check.NotNull(child, nameof(child));

            var newChildResult = child
               .Kernel
               .GetValidationResult(ValidationResultScope.All);

            bool joinedResultHasChanged = false;

            if (newChildResult.IsValid) {
               joinedResultHasChanged = RemoveCachedResult(child);
            } else {
               var childEntry = _entries.Find(x => x.Child == child);

               if (childEntry == null) {
                  childEntry = new ChildResultEntry(child);
                  _entries.Add(childEntry);
               }

               joinedResultHasChanged = !newChildResult.Equals(childEntry.Result);
               childEntry.Result = newChildResult;
            }

            if (joinedResultHasChanged) {
               RefreshJoinedResult();
            }

            return joinedResultHasChanged;
         }

         private void RefreshJoinedResult() {
            JoinedResult = ValidationResult.Join(
               _entries.Select(x => x.Result)
            );
         }

         private class ChildResultEntry {
            public ChildResultEntry(IViewModel child) {
               Child = child;
               Result = ValidationResult.Valid;
            }

            public IViewModel Child { get; private set; }
            public ValidationResult Result { get; set; }
         }
      }
   }
}
