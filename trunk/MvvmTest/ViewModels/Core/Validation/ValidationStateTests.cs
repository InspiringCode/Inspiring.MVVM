namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationStateTests {
      [TestMethod]
      public void DefaultValidInstance_AddError_ThrowsException() {
         AssertHelper.Throws<ArgumentException>(() =>
            ValidationState.Valid.Errors.Add(new ValidationError("Test"))
         );
      }

      [TestMethod]
      public void IsValid_NoErrors_ReturnsTrue() {
         var state = new ValidationState();
         Assert.IsTrue(state.IsValid);
      }

      [TestMethod]
      public void IsValid_OneError_ReturnsFalse() {
         var state = new ValidationState();
         state.Errors.Add(new ValidationError("Test"));
         Assert.IsFalse(state.IsValid);
      }
   }
}