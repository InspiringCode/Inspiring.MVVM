namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValidator {
      ValidationResult Execute(ValidationRequest request);
   }
}
