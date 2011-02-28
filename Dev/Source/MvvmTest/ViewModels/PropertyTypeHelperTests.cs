namespace Inspiring.MvvmTest.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PropertyTypeHelperTests : TestBase {
      [TestMethod]
      public void IsViewModel_Success() {
         Assert.IsTrue(PropertyTypeHelper.IsViewModel(typeof(EmployeeVM)));
      }

      [TestMethod]
      public void IsViewModelCollection_Success() {
         Assert.IsTrue(PropertyTypeHelper.IsViewModelCollection(typeof(VMCollection<EmployeeVM>)));
      }
   }
}