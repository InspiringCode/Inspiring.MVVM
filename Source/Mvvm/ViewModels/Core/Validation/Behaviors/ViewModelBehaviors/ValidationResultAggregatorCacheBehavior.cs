namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ValidationResultAggregatorCacheBehavior :
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
         Cache cache;

         switch (scope) {
            case ValidationResultScope.All:
               return ValidationResult.Join(
                  GetValidationResult(context, ValidationResultScope.Self),
                  GetValidationResult(context, ValidationResultScope.Descendants)
               );
            case ValidationResultScope.Self:
               return ValidationResult.Join(
                  GetValidationResult(context, ValidationResultScope.PropertiesOnly),
                  GetValidationResult(context, ValidationResultScope.ViewModelValidationsOnly)
               );
            case ValidationResultScope.Descendants:
               cache = GetCache(context);

               if (cache.DescendantResult == null) {
                  cache.DescendantResult = this.GetValidationResultNext(context, scope);
               }

               return cache.DescendantResult;
            case ValidationResultScope.ViewModelValidationsOnly:
               cache = GetCache(context);

               if (cache.ViewModelResult == null) {
                  cache.ViewModelResult = this.GetValidationResultNext(context, scope);
               }

               return cache.ViewModelResult;
            case ValidationResultScope.PropertiesOnly:
               cache = GetCache(context);

               if (cache.PropertyResult == null) {
                  cache.PropertyResult = this.GetValidationResultNext(context, scope);
               }

               return cache.PropertyResult;
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

         if (!args.ChangedPath.SelectsAncestor()) {
            bool validationResultChanged = args.ChangeType == ChangeType.ValidationResultChanged;

            bool collectionChanged =
               args.ChangeType == ChangeType.AddedToCollection ||
               args.ChangeType == ChangeType.RemovedFromCollection;

            // TODO: Is there a cleaner way? Should we introduce a own change type? Would maybe be useful!
            bool viewModelPropertyChanged = args.ChangedProperty != null ?
               PropertyTypeHelper.IsViewModel(args.ChangedProperty.PropertyType) :
               false;

            if (validationResultChanged || collectionChanged || viewModelPropertyChanged) {
               InvalidateCache(context);
               InvalidateParentCachesOf(context.VM);
            }
         }

         this.HandleChangedNext(context, args);
      }

      private void InvalidateCache(IBehaviorContext context) {
         GetCache(context).Invalidate();
      }

      private void InvalidateParentCachesOf(IViewModel vm) {
         foreach (IViewModel parent in vm.Kernel.Parents) {
            ValidationResultAggregatorCacheBehavior b;
            bool parentHasBehavior = parent
               .Descriptor
               .Behaviors
               .TryGetBehavior(out b);

            if (parentHasBehavior) {
               b.InvalidateCache(parent.GetContext());
            }

            InvalidateParentCachesOf(parent);
         }
      }

      private Cache GetCache(IBehaviorContext context) {
         if (!_cache.HasValue(context)) {
            _cache.Set(context, new Cache());
         }

         return _cache.Get(context);
      }

      private class Cache {
         public ValidationResult PropertyResult { get; set; }
         public ValidationResult ViewModelResult { get; set; }
         public ValidationResult DescendantResult { get; set; }

         public void Invalidate() {
            PropertyResult = null;
            ViewModelResult = null;
            DescendantResult = null;
         }
      }
   }
}
