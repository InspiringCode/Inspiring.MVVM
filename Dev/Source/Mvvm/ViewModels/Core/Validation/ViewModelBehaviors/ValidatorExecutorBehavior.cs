namespace Inspiring.Mvvm.ViewModels.Core.Validation.ViewModelBehaviors {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core.Validation.Validators;

   internal sealed class ValidatorExecutorBehavior :
      Behavior,
      IValidationExecutorBehavior {

      private CompositeValidator _compositeValidator = new CompositeValidator();

      public void AddValidator(IValidator validator) {
         Contract.Requires(validator != null);
         RequireNotSealed();

         _compositeValidator = _compositeValidator.Add(validator);
      }

      public ValidationResult Validate(IBehaviorContext context, ValidationRequest request) {
         Seal();
         return _compositeValidator.Execute(request);
      }
   }
}
