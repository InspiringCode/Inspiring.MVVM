namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValidationResultAggregatorBehavior : IBehavior {
      ValidationResult GetValidationResult(IBehaviorContext context, ValidationResultScope scope);
   }
}
