namespace Inspiring.Mvvm.ViewModels {
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

namespace Inspiring.Mvvm.ViewModels.Core {
   //public interface IValidationBehavior<TValue> {

   //}

   public interface IValidationBehavior {
      ValidationResult GetValidationResult(IBehaviorContext vm);
   }
}
