namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   public static class DomainAssert {
      public static void AreEqual(Path expected, Path actual) {
         if (!AreEqualCore(expected, actual)) {
            Assert.Fail("Expected {0} but was {1}.", expected, actual);
         }
      }

      public static void AreEqual(ValidationRequest expected, ValidationRequest actual) {
         if (!AreEqualCore(expected, actual)) {
            Assert.Fail("Expected {0} but was {1}.", expected, actual);
         }
      }

      public static void AreEqual(IEnumerable<ValidationRequest> expected, IEnumerable<ValidationRequest> actual) {
         if (!AreEqualCore(expected, actual)) {
            Assert.Fail(
               "Expected [{0}] but was [{1}].",
               String.Join(", ", expected),
               String.Join(", ", actual)
            );
         }
      }

      private static bool AreEqualCore(IEnumerable<ValidationRequest> expected, IEnumerable<ValidationRequest> actual) {
         if (expected.Count() != actual.Count()) {
            return false;
         }

         for (int i = 0; i < expected.Count(); i++) {
            if (!AreEqualCore(expected.ElementAt(i), actual.ElementAt(i))) {
               return false;
            }
         }

         return true;
      }

      private static bool AreEqualCore(ValidationRequest expected, ValidationRequest actual) {
         return
            actual.Step == expected.Step &&
            AreEqualCore(expected.TargetPath, actual.TargetPath);
      }

      private static bool AreEqualCore(Path expected, Path actual) {
         if (expected.Length != actual.Length) {
            return false;
         }

         for (int i = 0; i < expected.Length; i++) {
            if (!AreEqual(expected[i], actual[i])) {
               return false;
            }
         }

         return true;
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
         if (!AreEqualCore(expected, actual)) {
            Assert.Fail("Expected {0} but was {1}.", expected, actual);
         }
      }

      private static bool AreEqualCore(ChangeArgs expected, ChangeArgs actual) {
         if (Object.ReferenceEquals(expected, actual)) {
            return true;
         }

         if (expected == null || actual == null) {
            return false;
         }

         return
            expected.ChangeType == actual.ChangeType &&
            AreEqualCore(expected.ChangedPath, actual.ChangedPath) &&
            expected.OldItems.SequenceEqual(actual.OldItems) &&
            expected.NewItems.SequenceEqual(actual.NewItems);
      }

      private static bool AreEqualCore(IEnumerable<ChangeArgs> expected, IEnumerable<ChangeArgs> actual) {
         if (expected.Count() != actual.Count()) {
            return false;
         }


         for (int i = 0; i < expected.Count(); i++) {
            if (!AreEqualCore(expected.ElementAt(i), actual.ElementAt(i))) {
               return false;
            }
         }

         return true;
      }

      public static void AreEqual(IEnumerable<ChangeArgs> expected, IEnumerable<ChangeArgs> actual) {
         if (!AreEqualCore(expected, actual)) {
            Assert.Fail(
               "Expected [{0}] but was [{1}].",
               String.Join(", ", expected),
               String.Join(", ", actual)
            );
         }
      }

      private class ValidationErrorCO : ComparisonObject<ValidationError> {
         public ValidationErrorCO(ValidationError error)
            : base(error, x => x.Target.VM, x => x.Message) {
         }
      }
   }
}
