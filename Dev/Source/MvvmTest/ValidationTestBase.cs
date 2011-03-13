namespace Inspiring.MvvmTest {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationTestBase : TestBase {
      protected static ValidationState CreateValidationState(params string[] errors) {
         IEnumerable<ValidationState> states = errors
            .Select(error => new ValidationState(new ValidationError(error)));

         return ValidationState.Join(states);
      }
   }
}