namespace Inspiring.MvvmTest.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   internal class ValidationResultAggregatorStub :
      Behavior,
      IValidationResultAggregatorBehavior {

      public ValidationResultAggregatorStub() {
         ReturnedValidationResults = new Dictionary<ValidationResultScope, ValidationResult>();
      }

      public Dictionary<ValidationResultScope, ValidationResult> ReturnedValidationResults {
         get;
         set;
      }

      public ValidationResult GetValidationResult(IBehaviorContext context, ValidationResultScope scope) {
         ValidationResult result;
         return ReturnedValidationResults.TryGetValue(scope, out result) ?
            result :
            ValidationResult.Valid;
      }
   }
}
