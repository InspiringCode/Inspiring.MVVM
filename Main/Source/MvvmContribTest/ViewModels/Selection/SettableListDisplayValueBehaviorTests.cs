namespace Inspiring.MvvmContribTest.ViewModels.Selection {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SettableListDisplayValueBehaviorTests : TestBase {
      private SettableListDisplayValueBehavior<ItemVM> Behavior { get; set; }

      [TestMethod]
      public void TestMethod1() {
         // TODO: Test HashSet OutOfSync exception.
      }



      public class ItemVM : Inspiring.MvvmTest.ViewModels.ViewModelStub { }
   }
}