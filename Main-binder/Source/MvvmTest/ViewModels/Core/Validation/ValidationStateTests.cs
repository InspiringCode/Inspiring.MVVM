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

      [TestMethod]
      public void Equals_BothEmpty_ReturnsTrue() {
         var s1 = new ValidationState();
         var s2 = new ValidationState();

         Assert.IsTrue(s1.Equals(s2));
      }

      [TestMethod]
      public void Equals_SameErrorMessages_ReturnsTrue() {
         var s1 = new ValidationState();
         var s2 = new ValidationState();

         s1.Errors.Add(new ValidationError("Error 1"));
         s2.Errors.Add(new ValidationError("Error 1"));

         s1.Errors.Add(new ValidationError("Error 2"));
         s2.Errors.Add(new ValidationError("Error 2"));

         Assert.IsTrue(s1.Equals(s2));
      }

      [TestMethod]
      public void Equals_DifferentErrorMessages_ReturnsFalse() {
         var s1 = new ValidationState();
         var s2 = new ValidationState();

         s1.Errors.Add(new ValidationError("Error 1"));
         s2.Errors.Add(new ValidationError("Error 2"));

         Assert.IsFalse(s1.Equals(s2));
      }

      [TestMethod]
      public void Equals_DifferentNumberOfElements_ReturnsFalse() {
         var s1 = new ValidationState();
         var s2 = new ValidationState();

         s1.Errors.Add(new ValidationError("Error 1"));

         Assert.IsFalse(s1.Equals(s2));
      }
   }
}