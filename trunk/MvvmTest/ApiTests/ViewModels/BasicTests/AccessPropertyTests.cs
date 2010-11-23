namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class AccessPropertyTests {
      [TestMethod]
      public void TestMethod1() {
         AreaVM vm = new AreaVM(ServiceLocator.Current);
         vm.InitializeFrom(new Area("Test"));
         string value = vm.GetValue(AreaVM.Descriptor.Caption);
         Assert.AreEqual("Test", value);
      }
   }
}