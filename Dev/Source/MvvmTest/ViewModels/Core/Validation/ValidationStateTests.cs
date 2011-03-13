namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationStateTests : ValidationTestBase {
      [TestMethod]
      public void IsValid_NoErrors_ReturnsTrue() {
         var state = ValidationState.Valid;
         Assert.IsTrue(state.IsValid);
      }

      [TestMethod]
      public void IsValid_OneError_ReturnsFalse() {
         var state = CreateValidationState("Single error");
         Assert.IsFalse(state.IsValid);
      }

      [TestMethod]
      public void Equals_BothEmpty_ReturnsTrue() {
         var s1 = ValidationState.Valid;
         var s2 = ValidationState.Valid;

         Assert.IsTrue(s1.Equals(s2));
      }

      [TestMethod]
      public void Equals_SameErrorMessages_ReturnsTrue() {
         var s1 = CreateValidationState("Error 1", "Error 2");
         var s2 = CreateValidationState("Error 1", "Error 2");

         Assert.IsTrue(s1.Equals(s2));
      }

      [TestMethod]
      public void Equals_DifferentErrorMessages_ReturnsFalse() {
         var s1 = CreateValidationState("Error 1");
         var s2 = CreateValidationState("Error 2");

         Assert.IsFalse(s1.Equals(s2));
      }

      [TestMethod]
      public void Equals_DifferentNumberOfElements_ReturnsFalse() {
         var s1 = CreateValidationState("Error 1");
         var s2 = CreateValidationState();

         Assert.IsFalse(s1.Equals(s2));
      }


   }
}