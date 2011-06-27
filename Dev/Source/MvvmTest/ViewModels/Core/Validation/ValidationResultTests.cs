namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels;

   [TestClass]
   public class ValidationResultTests : ValidationTestBase {
      [TestMethod]
      public void IsValid_NoErrors_ReturnsTrue() {
         var state = ValidationResult.Valid;
         Assert.IsTrue(state.IsValid);
      }

      [TestMethod]
      public void IsValid_OneError_ReturnsFalse() {
         var state = CreateValidationResult("Single error");
         Assert.IsFalse(state.IsValid);
      }

      [TestMethod]
      public void Equals_BothEmpty_ReturnsTrue() {
         var s1 = ValidationResult.Valid;
         var s2 = ValidationResult.Valid;

         Assert.IsTrue(s1.Equals(s2));
      }

      [TestMethod]
      public void Equals_SameErrorMessages_ReturnsTrue() {
         var s1 = CreateValidationResult("Error 1", "Error 2");
         var s2 = CreateValidationResult("Error 1", "Error 2");

         Assert.IsTrue(s1.Equals(s2));
      }

      [TestMethod]
      public void Equals_DifferentErrorMessages_ReturnsFalse() {
         var s1 = CreateValidationResult("Error 1");
         var s2 = CreateValidationResult("Error 2");

         Assert.IsFalse(s1.Equals(s2));
      }

      [TestMethod]
      public void Equals_DifferentNumberOfElements_ReturnsFalse() {
         var s1 = CreateValidationResult("Error 1");
         var s2 = ValidationResult.Valid;

         Assert.IsFalse(s1.Equals(s2));
      }

      [TestMethod]
      public void Join_WhenResultsContainSameErrorInstance_FiltersDuplicateErrors() {
         var error = CreateValidationError();
         var s1 = CreateValidationResult(error);
         var s2 = CreateValidationResult(error);
         var joined = ValidationResult.Join(s1, s2);

         ValidationAssert.AreEqual(new ValidationResult(error), joined);
      }

      [TestMethod]
      public void Join_WhenResultsContainEqualErrors_DoesNotFiltersErrors() {
         var firstEqualError = CreateValidationError("Equal error");
         var secondEqualError = CreateValidationError("Equal error");
         Assert.AreEqual(firstEqualError, secondEqualError, "Assumption failed");

         var s1 = CreateValidationResult(firstEqualError);
         var s2 = CreateValidationResult(secondEqualError);

         var joined = ValidationResult.Join(s1, s2);
         var expected = new ValidationResult(new[] { firstEqualError, secondEqualError });
         
         ValidationAssert.AreEqual(expected, joined);
      }
   }
}