using Inspiring.Mvvm.Screens;
using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspiring.Mvvm.Common;
using System;

namespace Inspiring.MvvmTest.Screens {
   [TestClass]
   public class ScreenConductorTest : TestBase {
      private ScreenConductor Conductor { get; set; }
      private EventAggregator EventAggregator { get; set; }

      [TestInitialize]
      public void Setup() {
         EventAggregator = new EventAggregator();
         Conductor = new ScreenConductor(EventAggregator);
      }

      [TestMethod]
      public void OpenScreen_WhenScreenIsNotAlreadyOpen_PublishesScreenOpenedEvent() {
         var newScreen = new SingleInstanceScreen();

         var eventArgs = ExpectScreenOpenedEvent(() => {
            Conductor.OpenScreen(ScreenFactory.For(newScreen));
         });

         Assert.IsNotNull(eventArgs);
         Assert.AreEqual(Conductor, eventArgs.Conductor);
         Assert.AreEqual(newScreen, eventArgs.Screen);
         Assert.IsFalse(eventArgs.WasAlreadyOpen);
      }

      [TestMethod]
      public void OpenScreen_WhenScrenIsAlreadyOpen_PublishesScreenOpenedEvent() {
         var alreadyOpenedScreen = new SingleInstanceScreen();
         Conductor.OpenScreen(ScreenFactory.For(alreadyOpenedScreen));

         var eventArgs = ExpectScreenOpenedEvent(() => {
            Conductor.OpenScreen(ScreenFactory.For(alreadyOpenedScreen));
         });

         Assert.IsNotNull(eventArgs);
         Assert.AreEqual(Conductor, eventArgs.Conductor);
         Assert.AreEqual(alreadyOpenedScreen, eventArgs.Screen);
         Assert.IsTrue(eventArgs.WasAlreadyOpen);
      }

      private ScreenOpenedEventArgs ExpectScreenOpenedEvent(Action triggerAction) {
         ScreenOpenedEventArgs args = null;

         var sm = new EventSubscriptionManager(EventAggregator, b => {
            b.On(ScreenConductor.ScreenOpenedEvent).Execute(a => args = a);
         });

         triggerAction();

         sm.RemoveAllSubscriptions();
         return args;
      }

      [TestMethod]
      public void TestMethod1() {
         //ScreenConductor cd = new ScreenConductor();

         //var firstScreen = new Mock<ILifecycleHandler>(MockBehavior.Strict);
         //firstScreen.Setup(x => x.Initialize());
         //firstScreen.Setup(x => x.Activate());

         //var firstFactory = MockObjectFactory.MockScreenFactory(firstScreen.Object);

         //cd.OpenScreen(firstFactory.Object);

         //Assert.AreEqual(firstScreen.Object, cd.ActiveScreen);

         //firstFactory.Verify(x => x.Create(It.IsAny<IScreenInitializer>()), Times.Once());
         //firstScreen.Verify(x => x.Initialize(), Times.Once());
         //firstScreen.Verify(x => x.Activate(), Times.Once());

         //var secondScreen = new Mock<IScreen<string>>(MockBehavior.Strict);
         //secondScreen.Setup(x => x.Initialize());
         //secondScreen.Setup(x => x.Initialize("Test"));
         //secondScreen.Setup(x => x.Activate());

         //var secondFactory = MockObjectFactory.MockScreenFactory(secondScreen.Object, "Test");

         //firstScreen.Setup(x => x.Deactivate());

         //cd.OpenScreen(secondFactory.Object);
         //Assert.AreEqual(secondScreen.Object, cd.ActiveScreen);

         //secondFactory.Verify(x => x.Create(It.IsAny<IScreenInitializer>()), Times.Once());
         //secondScreen.Verify(x => x.Initialize(), Times.Once());
         //secondScreen.Verify(x => x.Initialize("Test"), Times.Once());
         //secondScreen.Verify(x => x.Activate(), Times.Once());
         //firstScreen.Verify(x => x.Deactivate());
      }



      [TestMethod]
      public void MockTest() {
         //var screen = new Mock<ILifecycleHandler>();


         //screen.Object.Initialize();
         //screen.Object.Activate();
         //screen.Object.Deactivate();

         //screen.Verify();
      }

      //private class Test : IScreenInitializer {

      //   public void Initialize(ILifecycleHandler screen) {

      //   }

      //   public void Initialize<TSubject>(IScreen<TSubject> screen, TSubject subject) {

      //   }
      //}

      [ScreenCreationBehavior(ScreenCreationBehavior.SingleInstance)]
      private class SingleInstanceScreen : ScreenBase {
      }
   }
}
