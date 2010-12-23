namespace Inspiring.MvvmTest.Screens {
   using System;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ScreenFactoryTest {
      [TestMethod]
      public void TestServiceLocatorIsUsed() {
         var locatorMock = new Mock<IServiceLocator>();
         locatorMock
            .Setup(x => x.GetInstance<TestScreen>())
            .Returns(new TestScreen("Test"));

         TestScreen screen = ScreenFactory
            .For<TestScreen>(locatorMock.Object)
            .Create();

         Assert.AreEqual("Test", screen.Dependency);
      }

      [TestMethod]
      public void TestCreateWithDefaultServiceLocator() {
         TestScreen screen = ScreenFactory
            .For<TestScreen>()
            .Create();

         Assert.IsNotNull(screen);
      }

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

         //var sf = ScreenFactory.WithSubject("Test").OfType<>().For<SubjectScreen>();
         //SubjectScreen s = sf.Create(mock.Object);

         //Assert.IsNotNull(s);
         //mock.Verify(x => x.Initialize(s), Times.Never());
         //mock.Verify(x => x.Initialize<string>(s, "Test"), Times.Once());
      }

      private class TestScreen : ScreenBase {
         public TestScreen() {
         }

         public TestScreen(string dependency) {
            Dependency = dependency;
         }

         public string Dependency { get; private set; }
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
