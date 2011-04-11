namespace Inspiring.MvvmTest {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   public static class DomainAssert {
      // TODO: Better ToString of path?
      public static void AreEqual(Path expected, Path actual) {
         if (expected.Length != actual.Length) {
            Assert.Fail("Expected {0} but was {1}.", expected, actual);
         }

         for (int i = 0; i < expected.Length; i++) {
            if (!AreEqual(expected[i], actual[i])) {
               Assert.Fail("Expected {0} but was {1}.", expected, actual);
            }
         }
      }

      public static void AreEqual(ValidationRequest expected, ValidationRequest actual) {
         Assert.AreEqual(expected.Step, actual.Step);
         DomainAssert.AreEqual(expected.TargetPath, actual.TargetPath);
      }

      private static bool AreEqual(PathStep first, PathStep second) {
         return first.Equals(second); // PathStep is a struct
      }

      public static void AreEqual(ValidationResult expected, ValidationResult actual) {
         var expectedErrors = expected.Errors.Select(x => new ValidationErrorCO(x)).ToArray();
         var actualErrors = actual.Errors.Select(x => new ValidationErrorCO(x)).ToArray();

         CollectionAssert.AreEquivalent(
            expectedErrors,
            actualErrors,
            "The two 'ValidationResult' objects contain different validation errors."
         );
      }

      public static void AreEqual(ChangeArgs expected, ChangeArgs actual) {
         if (Object.ReferenceEquals(expected, actual)) {
            return;
         }

         Assert.IsNotNull(expected);
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.ChangeType, actual.ChangeType);
         DomainAssert.AreEqual(expected.ChangedPath, actual.ChangedPath);
         CollectionAssert.AreEqual(expected.OldItems.ToArray(), actual.OldItems.ToArray());
         CollectionAssert.AreEqual(expected.NewItems.ToArray(), actual.NewItems.ToArray());
      }

      private class ValidationErrorCO : ComparisonObject<ValidationError> {
         public ValidationErrorCO(ValidationError error)
            : base(error, x => x.Message) {
         }
      }
   }
}
