namespace Inspiring.Mvvm.ViewModels.Behaviors {

   public interface IValidationBehavior<TValue> {

   }

   public interface IValidationBehavior {
      ValidationResult GetValidationResult(IBehaviorContext vm);
   }

   public sealed class ValidationResult {
      private ValidationResult() {
      }

      public bool Successful { get; private set; }

      public string ErrorMessage { get; private set; }

      public static ValidationResult Success() {
         return new ValidationResult { Successful = true };
      }

      public static ValidationResult Failure(string errorMessage) {
         return new ValidationResult { Successful = false, ErrorMessage = errorMessage };
      }
   }
}
