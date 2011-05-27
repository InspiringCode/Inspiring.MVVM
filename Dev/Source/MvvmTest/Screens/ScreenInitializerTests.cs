namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.Screens;

   [TestClass]
   public class ScreenInitializerTests {
      [TestMethod]
      public void Initialize_WithSubjectOfExactType_CallsInitialize() {
         var subject = new DerivedSubject();
         var screen = new TestScreen<DerivedSubject>();
         ScreenInitializer.Initialize(screen, subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WithSubjectOfDerivedType_CallsInitialize() {
         var subject = new DerivedSubject();
         var screen = new TestScreen<BaseSubject>();
         ScreenInitializer.Initialize(screen, subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WithSubjectOfDerivedType_CallsInitializeOnScreenAndItsChildren() {
         var subject = new DerivedSubject();
         var parent = new InitializableParentScreen<DerivedSubject>();
         var child = new TestScreen<BaseSubject>();
         parent.Children.Add(child);

         ScreenInitializer.Initialize(parent, subject);
         Assert.AreEqual(subject, parent.LastSubject, "Initialize was not called on parent.");
         Assert.AreEqual(subject, child.LastSubject, "Initialize was not called on child.");
      }

      [TestMethod]
      public void Initialize_WhenScreenDoesNotImplementInterface_CallsInitializeOnChildren() {
         var subject = new DerivedSubject();
         var parent = new NonInitializableParentScreen();
         var child = new TestScreen<BaseSubject>();
         parent.Children.Add(child);

         ScreenInitializer.Initialize(parent, subject);
         Assert.AreEqual(subject, child.LastSubject, "Initialize was not called on child.");
      }

      private class NonInitializableParentScreen : ScreenBase {
      }

      private class InitializableParentScreen<TSubject> : ScreenBase, INeedsInitialization<TSubject> {
         public TSubject LastSubject { get; set; }

         public void Initialize(TSubject subject) {
            LastSubject = subject;
         }
      }

      private class TestScreen<TSubject> : ScreenBase, INeedsInitialization<TSubject> {
         public TSubject LastSubject { get; set; }

         public void Initialize(TSubject subject) {
            LastSubject = subject;
         }
      }

      private class BaseSubject {
      }

      private class DerivedSubject : BaseSubject {
      }
   }
}