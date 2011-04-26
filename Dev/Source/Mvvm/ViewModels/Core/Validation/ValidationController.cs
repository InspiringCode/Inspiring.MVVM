namespace Inspiring.Mvvm.ViewModels.Core {

   public sealed class ValidationController {
      public void RequestPropertyRevalidation(
         IViewModel target,
         IVMPropertyDescriptor targetProperty
      ) {

      }

      public void RequestViewModelRevalidation(IViewModel target) {

      }

      public void ProcessPendingValidations() {

      }

      public ValidationResult GetResult(
         ValidationStep step,
         IViewModel target,
         IVMPropertyDescriptor targetProperty
      ) {
         return ValidationResult.Valid;
      }

      public void ManuallyBeginValidation() {

      }

      public void ManuallyEndValidation() {

      }
   }
}
