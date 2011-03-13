namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValidationStateProviderBehavior {
      ValidationResult GetValidationState(IBehaviorContext context);

      ValidationResult GetDescendantsValidationState(IBehaviorContext context);
   }
}
