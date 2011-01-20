namespace Inspiring.MvvmContribTest.ViewModels.Selection {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SettableListDisplayValueBehaviorTests {
      private SettableListDisplayValueBehavior<ItemVM> Behavior { get; set; }

      [TestMethod]
      public void TestMethod1() {
         // TODO: Test HashSet OutOfSync exception.
      }



      public class ItemVM : ViewModelStub { }
   }
}