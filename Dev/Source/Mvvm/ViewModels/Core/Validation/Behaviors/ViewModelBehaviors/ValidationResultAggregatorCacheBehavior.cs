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
         bool validationResultChanged = args.ChangeType == ChangeType.ValidationResultChanged;

         bool collectionChanged =
            args.ChangeType == ChangeType.AddedToCollection ||
            args.ChangeType == ChangeType.RemovedFromCollection;

         // TODO: Is there a cleaner way? Should we introduce a own change type? Would maybe be useful!
         bool viewModelPropertyChanged = args.ChangedProperty != null ?
            PropertyTypeHelper.IsViewModel(args.ChangedProperty.PropertyType) :
            false;

         if (validationResultChanged || collectionChanged || viewModelPropertyChanged) {
            GetCache(context).Invalidate();
         }

         this.HandleChangedNext(context, args);
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
