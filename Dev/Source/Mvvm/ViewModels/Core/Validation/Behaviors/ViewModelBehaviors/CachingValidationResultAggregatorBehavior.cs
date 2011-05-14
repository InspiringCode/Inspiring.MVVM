namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class CachingValidationResultAggregatorBehavior :
      Behavior,
      IBehaviorInitializationBehavior,
      IValidationResultAggregatorBehavior,
      IChangeHandlerBehavior {

      private DynamicFieldAccessor<Cache> _cache;

      public void Initialize(BehaviorInitializationContext context) {
         _cache = new DynamicFieldAccessor<Cache>(context, ViewModel.GeneralFieldGroup);
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

      private Cache GetCache(IBehaviorContext context) {
         Cache c;
         if (!_cache.TryGet(context, out c)) {
            c = new Cache();
            _cache.Set(context, c);
         }

         return c;
      }

      private void UpdateResultOfChild(IBehaviorContext context, IViewModel child) {
         GetCache(context).UpdateResultOfChild(context, child);
      }

      private class Cache {
         private readonly List<ChildResult> _childResults = new List<ChildResult>();

         public Cache() {
            PropertiesResult = ValidationResult.Valid;
            ViewModelResult = ValidationResult.Valid;
            DescendantResult = ValidationResult.Valid;
            SelfResult = ValidationResult.Valid;
            AllResult = ValidationResult.Valid;
         }

         public ValidationResult PropertiesResult { get; set; }
         public ValidationResult ViewModelResult { get; set; }
         public ValidationResult SelfResult { get; set; }
         public ValidationResult DescendantResult { get; set; }
         public ValidationResult AllResult { get; set; }

         public ValidationResult GetResult(ValidationResultScope scope) {
            switch (scope) {
               case ValidationResultScope.All:
                  return AllResult;
               case ValidationResultScope.Self:
                  return SelfResult;
               case ValidationResultScope.Descendants:
                  return DescendantResult;
               case ValidationResultScope.ViewModelValidationsOnly:
                  return ViewModelResult;
               case ValidationResultScope.PropertiesOnly:
                  return PropertiesResult;
               default:
                  throw new NotSupportedException();
            }
         }

         public void HandleChange(IBehaviorContext context, ChangeArgs args) {
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

            var self = context.VM;
            switch (args.ChangeType) {
               case ChangeType.PropertyChanged:
                  var ownPropertyChanged = args
                     .ChangedPath
                     .SelectsOnlyPropertyOf(self)
                     .Success;

                  if (ownPropertyChanged) {
                     args.OldItems.ForEach(x => RemoveCachedChildResult(context, x));
                     args.NewItems.ForEach(x => UpdateResultOfChild(context, x));
                  }
                  break;

                  //// TODO: Is there a cleaner way? Should we introduce a own change type? Would maybe be useful!
                  //bool viewModelPropertyChanged = args.ChangedProperty != null ?
                  //   PropertyTypeHelper.IsViewModel(args.ChangedProperty.PropertyType) :
                  //   false;

                  //if (viewModelPropertyChanged) {
                  //   var childVM = args
                  //      .ChangedVM
                  //      .Kernel
                  //      .GetValue(args.ChangedProperty) as IViewModel;

                  //   if (childVM != null) {
                  //      UpdateResultOfChild(context, childVM);
                  //   } else {
                  //      // TODO: How can we get the old VM?
                  //   }
                  //}

                  break;
               case ChangeType.ValidationResultChanged:
                  var ownViewModelResultChanged = args
                     .ChangedPath
                     .SelectsOnly(self)
                     .Success;

                  if (ownViewModelResultChanged) {
                     RefreshViewModelResult(context);
                     RefreshDerivedResults();
                     NotifyBehaviorOfParent(context);
                  } else {
                     var ownPropertyResultChanged = args
                        .ChangedPath
                        .SelectsOnlyPropertyOf(self)
                        .Success;

                     if (ownPropertyResultChanged) {
                        RefreshPropertiesResult(context);
                        RefreshDerivedResults();
                        NotifyBehaviorOfParent(context);
                     }
                  }
                  break;
               case ChangeType.AddedToCollection:
               case ChangeType.RemovedFromCollection:
                  var ownCollectionContentsChanged = args
                     .ChangedPath
                     .SelectsOnlyCollectionOf(self)
                     .Success;

                  if (ownCollectionContentsChanged) {
                     args.OldItems.ForEach(x => RemoveCachedChildResult(context, x));
                     args.NewItems.ForEach(x => UpdateResultOfChild(context, x));
                  }
                  break;
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

            PropertiesResult = ValidationResult.Join(results);
         }

         private void RefreshViewModelResult(IBehaviorContext context) {
            ViewModelResult = context
               .VM
               .Descriptor
               .Behaviors
               .GetValidationResultNext(context);
         }

         private void RefreshDerivedResults() {
            SelfResult = ValidationResult.Join(PropertiesResult, ViewModelResult);
            AllResult = ValidationResult.Join(SelfResult, DescendantResult);
         }

         private void NotifyBehaviorOfParent(IBehaviorContext context) {
            foreach (IViewModel parent in context.VM.Kernel.Parents) {
               CachingValidationResultAggregatorBehavior parentBehavior;
               bool parentHasBehavior = parent
                  .Descriptor
                  .Behaviors
                  .TryGetBehavior<CachingValidationResultAggregatorBehavior>(out parentBehavior);

               if (parentHasBehavior) {
                  parentBehavior.UpdateResultOfChild(parent.GetContext(), context.VM);
               }
            }
         }

         public void UpdateResultOfChild(IBehaviorContext context, IViewModel child) {
            var newChildResult = child
               .Kernel
               .GetValidationResult(ValidationResultScope.All);

            bool descendantsResultHasChanged = false;

            if (newChildResult.IsValid) {
               RemoveCachedChildResult(context, child);
            } else {
               var childCache = _childResults.Find(x => x.Child == child);
               if (childCache == null) {
                  childCache = new ChildResult(child);
                  _childResults.Add(childCache);
               }

               var oldChildResult = childCache.Result;
               childCache.Result = newChildResult;

               descendantsResultHasChanged = !newChildResult.Equals(oldChildResult);

               if (descendantsResultHasChanged) {
                  UpdateDescendantsResult(context);
               }
            }
         }

         private void RemoveCachedChildResult(IBehaviorContext context, IViewModel child) {
            bool childWasInvalidBefore = _childResults
               .RemoveAll(x => x.Child == child) > 0;

            if (childWasInvalidBefore) {
               UpdateDescendantsResult(context);
            }
         }

         private void UpdateDescendantsResult(IBehaviorContext context) {
            DescendantResult = ValidationResult.Join(
               _childResults.Select(x => x.Result)
            );

            RefreshDerivedResults();
            NotifyBehaviorOfParent(context);
         }

         private class ChildResult {
            public ChildResult(IViewModel child) {
               Child = child;
               Result = ValidationResult.Valid;
            }

            public IViewModel Child { get; private set; }
            public ValidationResult Result { get; set; }
         }
      }
   }
}
