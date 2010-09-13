﻿namespace Inspiring.MvvmTest.Screens {
   using System;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenFactoryTest {
      [TestMethod]
      public void ScreenIsCreatedAndInitialized() {
         //var mock = new Mock<IScreenInitializer>();

         //var sf = ScreenFactory.For<SimpleScreen>();
         //SimpleScreen s = sf.Create(mock.Object);

         //Assert.IsNotNull(s);
         //mock.Verify(x => x.Initialize(s), Times.Once());
      }

      [TestMethod]
      public void ScreenWithSubjectIsCreatedAndInitialized() {
         //var mock = new Mock<IScreenInitializer>();

         //var sf = ScreenFactory.WithSubject("Test").For<SubjectScreen>();
         //SubjectScreen s = sf.Create(mock.Object);

         //Assert.IsNotNull(s);
         //mock.Verify(x => x.Initialize(s), Times.Never());
         //mock.Verify(x => x.Initialize<string>(s, "Test"), Times.Once());
      }

      private class SimpleScreen : IScreen {
         public string Title {
            get { throw new NotImplementedException(); }
         }

         public void Initialize() {
            throw new NotImplementedException();
         }

         public void Activate() {
            throw new NotImplementedException();
         }

         public void Deactivate() {
            throw new NotImplementedException();
         }

         public void RequestClose() {
            throw new NotImplementedException();
         }

         public void Close() {
            throw new NotImplementedException();
         }


         public IScreenLifecycle Parent {
            get {
               throw new NotImplementedException();
            }
            set {
               throw new NotImplementedException();
            }
         }

         bool IScreenLifecycle.RequestClose() {
            throw new NotImplementedException();
         }
      }

      //private class SubjectScreen : SimpleScreen, IScreen<string> {
      //   public void Initialize(string subject) {
      //      throw new NotImplementedException();
      //   }
      //}
   }
}
