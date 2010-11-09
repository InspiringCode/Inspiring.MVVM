namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelValidatorHolderTests {
      [TestMethod]
      public void IsSealed_Initially_False() {
         ViewModelValidatorHolder holder = new ViewModelValidatorHolder();
         Assert.IsFalse(holder.IsSealed);
      }

      [TestMethod]
      public void GetValidators_SealsObject() {
         ViewModelValidatorHolder holder = new ViewModelValidatorHolder();
         holder.GetValidators();
         Assert.IsTrue(holder.IsSealed);
      }
   }
}
