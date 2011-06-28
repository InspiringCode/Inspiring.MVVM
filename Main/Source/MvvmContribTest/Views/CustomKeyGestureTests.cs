namespace Inspiring.MvvmContribTest.Views {
   using System;
   using System.Reflection;
   using System.Windows.Input;
   using Inspiring.Mvvm.Views;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CustomKeyGestureTests : TestBase {
      [TestMethod]
      public void Create_AssignsKey() {
         var g = CustomKeyGesture.Create(Key.A, ModifierKeys.None);
         Assert.AreEqual(Key.A, g.Key);
         Assert.AreEqual(ModifierKeys.None, g.Modifiers);
      }

      [TestMethod]
      public void Create_AssignsModifiers() {
         var g = CustomKeyGesture.Create(Key.A, ModifierKeys.Control);
         Assert.AreEqual(ModifierKeys.Control, g.Modifiers);
      }

      [TestMethod]
      public void DisplayString_NoModifiers_Success() {
         var g = CustomKeyGesture.Create(Key.A, ModifierKeys.None);
         Assert.AreEqual("A", g.DisplayString);
      }

      [TestMethod]
      public void DisplayString_SingleModifier_Success() {
         var g = CustomKeyGesture.Create(Key.A, ModifierKeys.Control);
         Assert.AreEqual("Ctrl+A", g.DisplayString);
      }

      [TestMethod]
      public void DisplayString_TwoModifiers_Success() {
         var g = CustomKeyGesture.Create(Key.A, ModifierKeys.Control | ModifierKeys.Shift);
         Assert.AreEqual("Ctrl+Shift+A", g.DisplayString);
      }

      [TestMethod]
      public void DisplayString_LocalizedModifiers_Success() {
         CustomKeyGesture.SupplyLocalization(KeyGestureLocalizations.ResourceManager);
         var g = CustomKeyGesture.Create(Key.A, ModifierKeys.Control | ModifierKeys.Shift);

         string expected = String.Format(
            "{0}+{1}+A",
            KeyGestureLocalizations.ModifierKeys_Control,
            KeyGestureLocalizations.ModifierKeys_Shift
         );

         Assert.AreEqual(expected, g.DisplayString);
      }

      [TestMethod]
      public void DisplayString_LocalizedKey_Success() {
         CustomKeyGesture.SupplyLocalization(KeyGestureLocalizations.ResourceManager);
         var g = CustomKeyGesture.Create(Key.Home, ModifierKeys.None);
         Assert.AreEqual(KeyGestureLocalizations.Key_Home, g.DisplayString);
      }

      [TestCleanup]
      public void Cleanup() {
         // Reset localization so that test do not influence each other!
         typeof(CustomKeyGesture)
            .GetField("_localizationResourceManager", BindingFlags.NonPublic | BindingFlags.Static)
            .SetValue(null, null);
      }
   }
}