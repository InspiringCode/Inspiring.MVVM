namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IDescendantsValidationResultProviderBehavior : IBehavior {
      ValidationResult GetDescendantsValidationResult(IBehaviorContext context);
   }
}
