namespace Inspiring.MvvmTest.Screens {
   using System.Linq;
   using Inspiring.Mvvm.Screens;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenTreeHelperTests : TestBase {
      [TestMethod]
      public void GetSelfAndChildren_WhenNoChildren_ReturnsOnlySelf() {
         var s = new Child();
         CollectionAssert.AreEqual(
            new[] { s },
            ScreenTreeHelper.GetChildrenOf(s, includeSelf: true).ToArray()
         );
      }

      [TestMethod]
      public void GetSelfAndChildren_WithChildren_ReturnsSelfAndChildren() {
         var p = new Parent();
         var c = new Child();
         p.Children.Attach(c);

         var result = ScreenTreeHelper.GetChildrenOf(p, includeSelf: true).ToArray();
         CollectionAssert.Contains(result, p);
         CollectionAssert.Contains(result, c);
      }

      [TestMethod]
      public void GetSelfAndChildren_WithGrandchildren_ReturnsSelfAndChildrenOnly() {
         var p = new Parent();
         var child = new Parent();
         var grand = new Child();
         p.Children.Attach(child);
         child.Children.Attach(grand);

         var result = ScreenTreeHelper.GetChildrenOf(p, includeSelf: true).ToArray();
         CollectionAssert.Contains(result, p);
         CollectionAssert.Contains(result, child);
         CollectionAssert.DoesNotContain(result, grand);
      }

      [TestMethod]
      public void GetDescendantsOfWithSelf_ReturnsSelfChildrenAndGrandchildren() {
         var p = new Parent();
         var child = new Parent();
         var grand = new Child();

         p.Children.Attach(child);
         child.Children.Attach(grand);

         var result = ScreenTreeHelper.GetDescendantsOf(p, includeSelf: true);

         CollectionAssert.AreEqual(
            new IScreenBase[] { p, child, grand },
            result.ToArray()
         );
      }

      [TestMethod]
      public void GetDescendantsOfWithoutSelf_ReturnsChildrenAndGrandchildren() {
         var p = new Parent();
         var child = new Parent();
         var grand = new Child();

         p.Children.Attach(child);
         child.Children.Attach(grand);

         var result = ScreenTreeHelper.GetDescendantsOf(p, includeSelf: false);

         CollectionAssert.AreEqual(
            new IScreenBase[] { child, grand },
            result.ToArray()
         );
      }
      
      private class Parent : DefaultTestScreen {
      }

      private class Child : DefaultTestScreen {
      }
   }
}
