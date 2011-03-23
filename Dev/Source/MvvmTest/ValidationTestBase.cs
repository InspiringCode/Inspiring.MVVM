namespace Inspiring.MvvmTest {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using ValidationArgs = Inspiring.Mvvm.ViewModels.Core.Validation.Validators.ValidationArgs;

   [TestClass]
   public class ValidationTestBase : TestBase {
      protected static ValidationResult CreateValidationState(params string[] errors) {
         IEnumerable<ValidationResult> states = errors
            .Select(error => new ValidationResult(new ValidationError(error)));

         return ValidationResult.Join(states);
      }

      protected static ValidationRequest CreateRequest(Path targetPath) {
         return new ValidationRequest(ValidationStep.Value, targetPath);
      }

      protected static void AssertErrors(ValidationArgs args, params ValidationError[] errors) {
         var expected = new ValidationResult(errors);
         Assert.AreEqual(expected, args.Result);
      }
   }
}