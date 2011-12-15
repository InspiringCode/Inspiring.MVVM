namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenConductorTest : ScreenLifecycleTestBase {
      private EventAggregator EventAggregator { get; set; }

      [TestInitialize]
      public void Setup() {
         EventAggregator = new EventAggregator();
      }

      [TestMethod]
      public void OpenScreen_WhenScreenIsNotAlreadyOpen_PublishesScreenOpenedEvent() {
         var conductor = CreateScreenConductor();
         var newScreen = new SingleInstanceScreen();

         var eventArgs = ExpectScreenOpenedEvent(() => {
            conductor.OpenScreen(ScreenFactory.For(newScreen));
         });

         Assert.IsNotNull(eventArgs);
         Assert.AreEqual(conductor, eventArgs.Conductor);
         Assert.AreEqual(newScreen, eventArgs.Screen);
         Assert.IsFalse(eventArgs.WasAlreadyOpen);
      }

      [TestMethod]
      public void OpenScreen_WhenScrenIsAlreadyOpen_PublishesScreenOpenedEvent() {
         var conductor = CreateScreenConductor();
         var alreadyOpenedScreen = new SingleInstanceScreen();
         conductor.OpenScreen(ScreenFactory.For(alreadyOpenedScreen));

         var eventArgs = ExpectScreenOpenedEvent(() => {
            conductor.OpenScreen(ScreenFactory.For(alreadyOpenedScreen));
         });

         Assert.IsNotNull(eventArgs);
         Assert.AreEqual(conductor, eventArgs.Conductor);
         Assert.AreEqual(alreadyOpenedScreen, eventArgs.Screen);
         Assert.IsTrue(eventArgs.WasAlreadyOpen);
      }

      [TestMethod]
      public void OpenScreen_WhenInitializeThrowsException_DoesNotAddScreenAndDoesNotChangeActiveScreen() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);

         ScreenMock newScreen = new ScreenMock { ThrowOnInitialize = true };
         OpenScreen(condcutor, newScreen);

         Assert.IsFalse(condcutor.Screens.Contains(newScreen));
         Assert.IsFalse(newScreen.WasActivated);
         Assert.AreEqual(alreadyOpen, condcutor.ActiveScreen);
         pc.AssertNoRaise();
      }

      [TestMethod]
      public void OpenScreen_WhenActivateThrowException_AddsScreenAndSetsActiveScreen() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);

         ScreenMock newScreen = new ScreenMock { ThrowOnActivate = true };
         OpenScreen(condcutor, newScreen);

         Assert.IsTrue(condcutor.Screens.Contains(newScreen));
         Assert.IsTrue(newScreen.WasInitialized);
         Assert.IsTrue(newScreen.WasActivated);
         Assert.AreEqual(newScreen, condcutor.ActiveScreen);
         pc.AssertOneRaise();
      }

      [TestMethod]
      public void ImmediateCloseScreen_WhenInitializeThrewException_DoesNotCallDeactivateAndCallsClose() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock newScreen = new ScreenMock { ThrowOnInitialize = true };

         OpenScreen(condcutor, newScreen);
         condcutor.ImmediateCloseScreen(newScreen);

         Assert.IsFalse(newScreen.WasDeactivated);
         Assert.IsTrue(newScreen.WasClosed);
      }

      [TestMethod]
      public void ImmediateCloseScreen_WhenActivateThrewException_CallsDeactivateAndClose() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock newScreen = new ScreenMock { ThrowOnActivate = true };

         OpenScreen(condcutor, newScreen);
         condcutor.ImmediateCloseScreen(newScreen);

         Assert.IsTrue(newScreen.WasDeactivated);
         Assert.IsTrue(newScreen.WasClosed);
      }

      [TestMethod]
      public void ImmediateCloseScreen_WhenInitializeThrewException_DoesNotChangeActiveScreen() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);

         ScreenMock newScreen = new ScreenMock { ThrowOnInitialize = true };
         OpenScreen(condcutor, newScreen);

         condcutor.ImmediateCloseScreen(newScreen);

         Assert.IsFalse(condcutor.Screens.Contains(newScreen));
         Assert.AreEqual(alreadyOpen, condcutor.ActiveScreen);
         pc.AssertNoRaise();
      }

      [TestMethod]
      public void ImmediateCloseScreen_WhenActivateThrewException_SetsActiveScreenToLastActive() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);


         ScreenMock newScreen = new ScreenMock { ThrowOnActivate = true };
         OpenScreen(condcutor, newScreen);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);
         condcutor.ImmediateCloseScreen(newScreen);

         Assert.IsFalse(condcutor.Screens.Contains(newScreen));
         Assert.AreEqual(alreadyOpen, condcutor.ActiveScreen);
         pc.AssertOneRaise();
      }

      [TestMethod]
      public void CloseScreen_WhenDeactivateThrowsException_RemovesScreen() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);

         ScreenMock newScreen = new ScreenMock { ThrowOnDeactivate = true };
         OpenScreen(condcutor, newScreen);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);
         CloseScreen(condcutor, newScreen);

         Assert.IsFalse(newScreen.WasClosed);
         Assert.IsFalse(condcutor.Screens.Contains(newScreen));
         Assert.AreEqual(alreadyOpen, condcutor.ActiveScreen);
         pc.AssertOneRaise();
      }

      [TestMethod]
      public void CloseScreen_WhenCloseThrowsException_RemovesScreen() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);

         ScreenMock newScreen = new ScreenMock { ThrowOnClose = true };
         OpenScreen(condcutor, newScreen);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);
         CloseScreen(condcutor, newScreen);

         Assert.IsFalse(condcutor.Screens.Contains(newScreen));
         Assert.AreEqual(alreadyOpen, condcutor.ActiveScreen);
         pc.AssertOneRaise();
      }

      private ScreenConductor CreateScreenConductor() {
         ScreenConductor conductor = new ScreenConductor(EventAggregator);
         IScreenBase s = conductor;
         s.Activate();
         return conductor;
      }

      private ScreenMock OpenNewScreen(ScreenConductor conductor) {
         ScreenMock screen = new ScreenMock();
         OpenScreen(conductor, screen);
         return screen;
      }

      private void OpenScreen(ScreenConductor conductor, IScreenBase screen) {
         try {
            conductor.OpenScreen(ScreenFactory.For(screen));
         } catch (ScreenMockException) {
         }
      }

      private void CloseScreen(ScreenConductor conductor, IScreenBase screen) {
         try {
            conductor.CloseScreen(screen);
         } catch (ScreenMockException) {
         }
      }

      private PropertyChangedCounter CreateActiveScreenChangedListener(ScreenConductor conductor) {
         return new PropertyChangedCounter(conductor, "ActiveScreen");
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

      [ScreenCreationBehavior(ScreenCreationBehavior.SingleInstance)]
      private class SingleInstanceScreen : ScreenBase {
      }
   }
}
