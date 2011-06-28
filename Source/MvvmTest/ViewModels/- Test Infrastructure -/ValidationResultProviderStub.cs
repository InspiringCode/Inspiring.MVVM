namespace Inspiring.MvvmTest.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class ValidationResultProviderStub :
      Behavior,
      IValidationResultProviderBehavior,
      IDescendantsValidationResultProviderBehavior {

      public ValidationResultProviderStub() {
         ReturnedResult = ValidationResult.Valid;
         ReturnedDescendantsResult = ValidationResult.Valid;
      }

      public ValidationResult ReturnedResult { get; set; }

      public ValidationResult ReturnedDescendantsResult { get; set; }

      public ValidationResult GetValidationResult(IBehaviorContext context) {
         return ReturnedResult;
      }

      public ValidationResult GetDescendantsValidationResult(IBehaviorContext context) {
         return ReturnedDescendantsResult;
      }
   }
}
