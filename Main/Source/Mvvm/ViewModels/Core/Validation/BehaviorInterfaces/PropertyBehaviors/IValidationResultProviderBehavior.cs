namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValidationResultProviderBehavior {
      ValidationResult GetValidationResult(IBehaviorContext context);
   }
}
