namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ViewModelValidationSourceBehavior :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IValidationStateProviderBehavior,
      IRevalidationBehavior {

      private ValidationResultManager _resultManager;

      public void Initialize(BehaviorInitializationContext context) {
         _resultManager = new ValidationResultManager(context, ViewModel.GeneralFieldGroup);
         SetInitialized();
      }

      public void Revalidate(IBehaviorContext context, CollectionResultCache cache) {
         RequireInitialized();

         var result = ValidationOperation.PerformViewModelValidation(cache, context.VM);
         _resultManager.UpdateValidationResult(context, result);
      }

      public void Revalidate(IBehaviorContext context, ValidationContext validationContext, ValidationMode mode) {
         throw new NotImplementedException();
      }

      public void Revalidate(IBehaviorContext context) {
         throw new NotImplementedException();
      }

      public ValidationResult GetValidationState(IBehaviorContext context) {
         RequireInitialized();
         return _resultManager.GetValidationResult(context);
      }

      public ValidationResult GetDescendantsValidationState(IBehaviorContext context) {
         throw new NotImplementedException();
      }
   }
}
