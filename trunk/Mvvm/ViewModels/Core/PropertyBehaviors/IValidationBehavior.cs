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

      public override int GetHashCode() {
         return Successful.GetHashCode() ^ ErrorMessage.GetHashCode();
      }

      public override bool Equals(object obj) {
         ValidationResult other = obj as ValidationResult;
         return
            other != null &&
            other.Successful == Successful &&
            other.ErrorMessage == ErrorMessage;
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
