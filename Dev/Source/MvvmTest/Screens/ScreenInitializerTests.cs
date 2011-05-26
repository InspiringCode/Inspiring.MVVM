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

      private class TestScreen<TSubject> : ScreenLifecycle, INeedsInitialization<TSubject> {
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