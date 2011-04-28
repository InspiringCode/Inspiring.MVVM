namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ViewModelValidationSourceBehavior :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IChangeHandlerBehavior,
      IValidationResultProviderBehavior,
      IViewModelRevalidationBehavior,
      IRefreshControllerBehavior {

      private ValidationResultManager _resultManager;

      public void Initialize(BehaviorInitializationContext context) {
         _resultManager = new ValidationResultManager(context, ViewModel.GeneralFieldGroup);
         SetInitialized();

         this.InitializeNext(context);
      }

      public void Revalidate(IBehaviorContext context, ValidationController controller) {
         RequireInitialized();

         ValidationResult result = controller.GetResult(ValidationStep.ViewModel, context.VM);
         _resultManager.UpdateValidationResult(context, result);

         this.ViewModelRevalidateNext(context, controller);
      }

      public void Refresh(IBehaviorContext context) {
         this.ViewModelRefreshNext(context);
         Revalidator.RevalidateViewModelValidations(context.VM);
      }

      public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property) {
         this.ViewModelRefreshNext(context, property);
      }

      public ValidationResult GetValidationResult(IBehaviorContext context) {
         RequireInitialized();

         return ValidationResult.Join(
            _resultManager.GetValidationResult(context),
            this.GetValidationResultNext(context)
         );
      }

      public void HandleChange(IBehaviorContext context, ChangeArgs args) {
         this.HandleChangedNext(context, args);
         Revalidator.RevalidateViewModelValidations(context.VM);
      }
   }
}
