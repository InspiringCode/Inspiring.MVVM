namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationErrorCollectionTests : TestBase {
      [TestMethod]
      public void DefaultEmptyInstance_AddError_ThrowsException() {
         AssertHelper.Throws<ArgumentException>(() =>
            ValidationErrorCollection.Empty.Add(new ValidationError("Test"))
         );
      }
   }
}