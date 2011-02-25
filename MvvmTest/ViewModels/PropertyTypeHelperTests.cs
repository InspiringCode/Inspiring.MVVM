namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels;

   [TestClass]
   public class PropertyTypeHelperTests {
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