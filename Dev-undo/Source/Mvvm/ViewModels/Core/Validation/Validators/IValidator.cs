namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {

   public interface IValidator {
      ValidationResult Execute(ValidationRequest request);
   }
}
