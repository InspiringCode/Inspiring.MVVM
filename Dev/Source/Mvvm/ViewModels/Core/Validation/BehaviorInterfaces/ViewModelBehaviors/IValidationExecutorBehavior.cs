namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels.Core.Validation;

   public interface IValidationExecutorBehavior : IBehavior {
      ValidationResult Validate(IBehaviorContext context, ValidationRequest request);
   }
}
