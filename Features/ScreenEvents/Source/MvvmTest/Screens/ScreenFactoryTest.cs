namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ScreenFactoryTest : TestBase {
      private EventAggregator Aggregator { get; set; }
      private ServiceLocatorStub Locator { get; set; }

      [TestInitialize]
      public void Setup() {
         Aggregator = new EventAggregator();
         Locator = new ServiceLocatorStub();
         Locator.Register<InitializableScreen>(() => new InitializableScreen(Aggregator));
      }

      [TestMethod]
      public void TestServiceLocatorIsUsed() {
         var locatorMock = new Mock<IServiceLocator>();
         locatorMock
            .Setup(x => x.GetInstance<TestScreen>())
            .Returns(new TestScreen("Test"));

         TestScreen screen = ScreenFactory
            .For<TestScreen>(locatorMock.Object)
            .Create(Aggregator);

         Assert.AreEqual("Test", screen.Dependency);
      }

      [TestMethod]
      public void TestCreateWithDefaultServiceLocator() {
         TestScreen screen = ScreenFactory
            .For<TestScreen>()
            .Create(Aggregator);

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

      [TestMethod]
      public void ScreenFactoriesForConcreteScree_CanBeAssignedToScreenBaseFactoryVariable() {
         IScreenFactory<TestScreen> concreteFactory = ScreenFactory.For<TestScreen>();
         IScreenFactory<IScreenBase> factory = concreteFactory;

         IScreenBase screen = factory.Create(Aggregator);
         Assert.IsInstanceOfType(screen, typeof(TestScreen));
      }

      [TestMethod]
      public void Create_ScreenFactoryForInstance_CallsInitialize() {
         var expected = new InitializableScreen(Aggregator);

         IScreenFactory<InitializableScreen> factory = ScreenFactory.For<InitializableScreen>(Locator);

         var actual = factory.Create(Aggregator);
         Assert.IsTrue(actual.InitializeWasCalled);
      }

      [TestMethod]
      public void Create_ScreenFactoryForInstance_ReturnsThisInstance() {
         var expected = new InitializableScreen(Aggregator);

         IScreenFactory<InitializableScreen> factory = ScreenFactory.For(expected);

         var actual = factory.Create(Aggregator);
         Assert.AreEqual(expected, actual);
      }

      private class InitializableScreen : DefaultTestScreen {
         public InitializableScreen(EventAggregator aggregator)
            : base(aggregator) {

            Lifecycle.RegisterHandler(
               ScreenEvents.Initialize(),
               args => InitializeWasCalled = true
            );
         }

         public bool InitializeWasCalled { get; set; }
      }

      private class FirstScreen : DefaultTestScreen {
      }

      private class SecondScreen : DefaultTestScreen {
      }

      private class TestScreen : DefaultTestScreen {
         public TestScreen() {
         }

         public TestScreen(string dependency) {
            Dependency = dependency;
         }

         public string Dependency { get; private set; }
      }

      private class SimpleScreen : IScreenBase {
         public IScreenBase Parent {
            get {
               throw new NotImplementedException();
            }
            set {
               throw new NotImplementedException();
            }
         }


         ICollection<object> IScreenBase.Children {
            get { throw new NotImplementedException(); }
         }
      }

      //private class SubjectScreen : SimpleScreen, IScreen<string> {
      //   public void Initialize(string subject) {
      //      throw new NotImplementedException();
      //   }
      //}
   }
}
