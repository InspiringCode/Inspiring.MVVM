namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValidationExecutorBehavior : IBehavior {
      ValidationResult Validate(IBehaviorContext context, ValidationRequest request);
   }
}
