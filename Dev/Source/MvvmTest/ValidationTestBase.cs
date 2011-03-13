namespace Inspiring.MvvmTest {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationTestBase : TestBase {
      protected static ValidationResult CreateValidationState(params string[] errors) {
         IEnumerable<ValidationResult> states = errors
            .Select(error => new ValidationResult(new ValidationError(error)));

         return ValidationResult.Join(states);
      }
   }
}