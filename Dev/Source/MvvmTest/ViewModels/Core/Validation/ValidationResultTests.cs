namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

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


   }
}