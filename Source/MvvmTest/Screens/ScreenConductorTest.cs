namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenConductorTest : ScreenLifecycleTestBase {
      private EventAggregator Aggregator { get; set; }

      [TestInitialize]
      public void Setup() {
         Aggregator = new EventAggregator();
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
      public void CloseScreen_WhenScreenIsNotContained_ThrowsArgumentException() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock uncontainedScreen = new ScreenMock(Aggregator);

         AssertHelper.Throws<ArgumentException>(() =>
            condcutor.CloseScreen(uncontainedScreen, requestClose: true)
         );
      }

      [TestMethod]
      public void OpenScreen_WhenInitializeThrowsException_DoesNotAddScreenAndDoesNotChangeActiveScreen() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);

         ScreenMock newScreen = new ScreenMock(Aggregator) { ThrowOnInitialize = true };
         OpenScreen(condcutor, newScreen);

         Assert.IsFalse(newScreen.WasActivated);
         Assert.IsFalse(condcutor.Screens.Contains(newScreen));
         Assert.AreEqual(alreadyOpen, condcutor.ActiveScreen);
         pc.AssertNoRaise();
      }

      [TestMethod]
      public void OpenScreen_WhenActivateThrowsException_DoesNotAddScreenAndSetsActivateScreenToNull() {
         ScreenConductor conductor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(conductor);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(conductor);

         ScreenMock newScreen = new ScreenMock(Aggregator) { ThrowOnActivate = true };
         OpenScreen(conductor, newScreen);

         Assert.IsTrue(newScreen.WasInitialized);
         Assert.IsTrue(newScreen.WasActivated);
         Assert.IsFalse(conductor.Screens.Contains(newScreen));
         Assert.IsNull(conductor.ActiveScreen);
         pc.AssertOneRaise();
      }

      [TestMethod]
      public void OpenScreen_WhenDeactivateThrowsException_AddsScreenAndUpdatesActiveScreen() {
         ScreenConductor conductor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(conductor);

         ScreenMock oldScreen = new ScreenMock(Aggregator) { ThrowOnDeactivate = true };
         OpenScreen(conductor, oldScreen);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(conductor);

         ScreenMock newScreen = new ScreenMock(Aggregator);
         OpenScreen(conductor, newScreen);

         Assert.IsTrue(newScreen.WasInitialized);
         Assert.IsTrue(newScreen.WasActivated);
         Assert.IsTrue(conductor.Screens.Contains(newScreen));
         Assert.AreEqual(newScreen, conductor.ActiveScreen);

         Assert.IsFalse(conductor.Screens.Contains(oldScreen));

         pc.AssertOneRaise();
      }

      [TestMethod]
      public void CloseScreen_WhenActivateOfPreviouslyActiveThrowsException_RemovesPreviouslyActiveAndSetsActiveScreenToNull() {
         ScreenConductor conductor = CreateScreenConductor();
         OpenNewScreen(conductor);

         ScreenMock previouslyActive = new ScreenMock(Aggregator);
         OpenScreen(conductor, previouslyActive);

         ScreenMock newScreen = new ScreenMock(Aggregator);
         OpenScreen(conductor, newScreen);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(conductor);

         previouslyActive.ThrowOnActivate = true;

         CloseScreen(conductor, newScreen, false);

         Assert.IsFalse(conductor.Screens.Contains(newScreen));
         Assert.IsFalse(conductor.Screens.Contains(previouslyActive));
         Assert.IsNull(conductor.ActiveScreen);
         pc.AssertOneRaise();
      }

      [TestMethod]
      public void CloseScreen_WhenRequestCloseThrowsException_RemovesScreen() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);

         ScreenMock newScreen = new ScreenMock(Aggregator) { ThrowOnRequestClose = true };
         OpenScreen(condcutor, newScreen);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);

         CloseScreen(condcutor, newScreen, false);

         Assert.IsFalse(condcutor.Screens.Contains(newScreen));
         Assert.AreEqual(alreadyOpen, condcutor.ActiveScreen);
         pc.AssertOneRaise();
      }
      
      [TestMethod]
      public void CloseScreen_WhenDeactivateThrowsException_RemovesScreen() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);

         ScreenMock newScreen = new ScreenMock(Aggregator) { ThrowOnDeactivate = true };
         OpenScreen(condcutor, newScreen);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);

         CloseScreen(condcutor, newScreen, false);

         Assert.IsFalse(condcutor.Screens.Contains(newScreen));
         Assert.AreEqual(alreadyOpen, condcutor.ActiveScreen);
         pc.AssertOneRaise();
      }

      [TestMethod]
      public void CloseScreen_WhenCloseThrowsException_RemovesScreen() {
         ScreenConductor condcutor = CreateScreenConductor();
         ScreenMock alreadyOpen = OpenNewScreen(condcutor);

         ScreenMock newScreen = new ScreenMock(Aggregator) { ThrowOnClose = true };
         OpenScreen(condcutor, newScreen);

         PropertyChangedCounter pc = CreateActiveScreenChangedListener(condcutor);
         CloseScreen(condcutor, newScreen, false);

         Assert.IsFalse(condcutor.Screens.Contains(newScreen));
         Assert.AreEqual(alreadyOpen, condcutor.ActiveScreen);
         pc.AssertOneRaise();
      }

      [TestMethod]
      public void RequestClose_StopsAfterFirstChildReturnsFalse() {
         ScreenConductor conductor = CreateScreenConductor();
         var first = new ScreenMock(Aggregator) { RequestCloseResult = true };
         var second = new ScreenMock(Aggregator) { RequestCloseResult = false };
         var third = new ScreenMock(Aggregator) { RequestCloseResult = true };

         OpenScreen(conductor, first);
         OpenScreen(conductor, second);
         OpenScreen(conductor, third);

         bool result = new ScreenLifecycleOperations(Aggregator, conductor)
            .RequestClose();

         Assert.IsFalse(result, "RequestClose should return false.");
         Assert.IsTrue(first.WasCloseRequested);
         Assert.IsTrue(second.WasCloseRequested);
         Assert.IsFalse(third.WasCloseRequested);
      }

      [TestMethod]
      public void CloseScreen_SetsParentToNull() {
         ScreenConductor conductor = CreateScreenConductor();
         ScreenMock s = new ScreenMock(Aggregator);

         OpenScreen(conductor, s);
         Assert.AreEqual(conductor, s.Parent);

         CloseScreen(conductor, s, false);
         Assert.IsNull(s.Parent);
      }

      private ScreenConductor CreateScreenConductor() {
         ScreenConductor conductor = ScreenFactory
            .For(new ScreenConductor(Aggregator))
            .Create(Aggregator);

         new ScreenLifecycleOperations(Aggregator, conductor)
            .Activate();

         return conductor;
      }

      private ScreenMock OpenNewScreen(ScreenConductor conductor) {
         ScreenMock screen = new ScreenMock(Aggregator);
         OpenScreen(conductor, screen);
         return screen;
      }

      private void OpenScreen(ScreenConductor conductor, IScreenBase screen) {
         try {
            conductor.OpenScreen(ScreenFactory.For(screen));
         } catch (ScreenLifecycleException) {
         }
      }

      private void CloseScreen(ScreenConductor conductor, IScreenBase screen, bool skipRequestClose) {
         try {
            conductor.CloseScreen(screen, skipRequestClose);
         } catch (ScreenLifecycleException) {
         }
      }

      private PropertyChangedCounter CreateActiveScreenChangedListener(ScreenConductor conductor) {
         return new PropertyChangedCounter(conductor, "ActiveScreen");
      }

      private ScreenOpenedEventArgs ExpectScreenOpenedEvent(Action triggerAction) {
         ScreenOpenedEventArgs args = null;

         var sm = new EventSubscriptionManager(Aggregator, b => {
            b.On(ScreenConductor.ScreenOpenedEvent).Execute(a => args = a);
         });

         triggerAction();

         sm.RemoveAllSubscriptions();
         return args;
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.SingleInstance)]
      private class SingleInstanceScreen : DefaultTestScreen {
      }
   }
}
