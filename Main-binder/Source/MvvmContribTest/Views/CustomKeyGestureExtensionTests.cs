namespace Inspiring.MvvmContribTest.Views {
   using System.Windows.Input;
   using Inspiring.Mvvm.Views;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CustomKeyGestureExtensionTests {
      [TestMethod]
      public void ProvideValue_WithModifierKey() {
         var e = new CustomKeyGestureExtension("Ctrl+A");
         var g = (KeyGesture)e.ProvideValue(null);
         Assert.AreEqual(Key.A, g.Key);
         Assert.AreEqual(ModifierKeys.Control, g.Modifiers);
      }

      [TestMethod]
      public void ProvideValue_AlphanumericKeyWithoutModifier() {
         Assert.Inconclusive("Still todo");
         var e = new CustomKeyGestureExtension("A");
         var g = (KeyGesture)e.ProvideValue(null);
         Assert.AreEqual(Key.A, g.Key);
         Assert.AreEqual(ModifierKeys.None, g.Modifiers);
      }
   }
}