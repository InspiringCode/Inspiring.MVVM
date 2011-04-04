namespace Inspiring.MvvmTest {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   public static class DomainAssert {
      public static void AreEqual(ValidationResult expected, ValidationResult actual) {
         var expectedErrors = expected.Errors.Select(x => new ValidationErrorCO(x)).ToArray();
         var actualErrors = actual.Errors.Select(x => new ValidationErrorCO(x)).ToArray();

         CollectionAssert.AreEquivalent(
            expectedErrors,
            actualErrors,
            "The two 'ValidationResult' objects contain different validation errors."
         );
      }

      private class ValidationErrorCO : ComparisonObject<ValidationError> {
         public ValidationErrorCO(ValidationError error)
            : base(error, x => x.Message) {
         }
      }
   }
}
