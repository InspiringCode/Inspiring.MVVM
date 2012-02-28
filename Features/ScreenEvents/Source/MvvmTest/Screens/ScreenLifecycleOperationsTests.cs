namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenLifecycleOperationsTests {
      private EventSubscriptionManager _sm;
      private EventAggregator Aggregator { get; set; }
      private List<IEvent> PublishedEvents { get; set; }

      [TestInitialize]
      public void Setup() {
         Aggregator = new EventAggregator();
         _sm = new EventSubscriptionManager(Aggregator);
      }

      [TestMethod]
      public void VariousOperations_PublishCorrectEvents() {
         ParameterizedTest
            .TestCase<Action<ScreenLifecycleOperations>, IEvent>(x => x.Initialize(), ScreenEvents.Initialize())
            .TestCase(x => x.Initialize(new Subject()), ScreenEvents.Initialize<Subject>())
            .TestCase(x => x.Activate(), ScreenEvents.Activate)
            .TestCase(x => x.Deactivate(), ScreenEvents.Deactivate)
            .TestCase(x => x.RequestClose(), ScreenEvents.RequestClose)
            .TestCase(x => x.Close(), ScreenEvents.Close)
            .Run((lifecycleAction, expectedEvent) => {
               List<IEvent> actualEvents = new List<IEvent>();
               TestScreen screen = new TestScreen();
               ScreenLifecycleOperations ops = new ScreenLifecycleOperations(Aggregator, screen);

               AddEventHandlerForAllEvents(
                  screen,
                  handlerAction: actualEvents.Add,
                  includeExceptionOccured: true
               );

               lifecycleAction(ops);

               IEvent actualEvent = actualEvents.SingleOrDefault();
               Assert.AreEqual(expectedEvent, actualEvent);
            });
      }

      [TestMethod]
      public void VariousOperations_WhenHandlerThrowsException_RaiseLifecycleExceptionOccuredEventAndThrowLifecycleException() {
         ParameterizedTest
            .TestCase<Action<ScreenLifecycleOperations>, IEvent>(x => x.Initialize(), ScreenEvents.Initialize())
            .TestCase(x => x.Initialize(new Subject()), ScreenEvents.Initialize<Subject>())
            .TestCase(x => x.Activate(), ScreenEvents.Activate)
            .TestCase(x => x.Deactivate(), ScreenEvents.Deactivate)
            .TestCase(x => x.RequestClose(), ScreenEvents.RequestClose)
            .TestCase(x => x.Close(), ScreenEvents.Close)
            .Run((lifecycleAction, expectedEvent) => {
               List<IEvent> actualEvents = new List<IEvent>();
               TestScreen screen = new TestScreen();
               ScreenLifecycleOperations ops = new ScreenLifecycleOperations(Aggregator, screen);

               InvalidOperationException sourceException = new InvalidOperationException();

               AddEventHandlerForAllEvents(
                  screen,
                  handlerAction: e => {
                     actualEvents.Add(e);
                     throw sourceException;
                  },
                  includeExceptionOccured: false
               );

               AddEventHandlerFor(
                  ScreenEvents.LifecycleExceptionOccured,
                  screen,
                  handlerAction: (ev, _) => actualEvents.Add(ev)
               );

               var exceptionExpr = AssertHelper.Throws<ScreenLifecycleException>(() =>
                  lifecycleAction(ops)
               );

               CollectionAssert.AreEquivalent(
                  new IEvent[] { 
                     expectedEvent,
                     ScreenEvents.LifecycleExceptionOccured
                  },
                  actualEvents
               );

               Assert.AreEqual(sourceException, exceptionExpr.Exception.InnerException);
            });
      }

      [TestMethod]
      public void RequestClose_ReturnsTrueIfNoHandlerSetsIsCloseAllowedToFalse() {
         TestScreen screen = new TestScreen();
         ScreenLifecycleOperations ops = new ScreenLifecycleOperations(Aggregator, screen);

         bool result = new ScreenLifecycleOperations(Aggregator, screen)
            .RequestClose();

         Assert.IsTrue(result);

         AddEventHandlerFor(
            ScreenEvents.RequestClose,
            screen,
            (ev, args) => { }
         );

         result = new ScreenLifecycleOperations(Aggregator, screen)
            .RequestClose();

         Assert.IsTrue(result);

         AddEventHandlerFor(
            ScreenEvents.RequestClose,
            screen,
            (ev, args) => { args.IsCloseAllowed = false; }
         );

         result = new ScreenLifecycleOperations(Aggregator, screen)
            .RequestClose();

         Assert.IsFalse(result);
      }

      private void AddEventHandlerForAllEvents(
         IScreenBase target,
         Action<IEvent> handlerAction,
         bool includeExceptionOccured = false
      ) {
         Action<IEvent, object> action = (ev, _) => handlerAction(ev);

         AddEventHandlerFor(ScreenEvents.Initialize(), target, action);
         AddEventHandlerFor(ScreenEvents.Initialize<Subject>(), target, action);
         AddEventHandlerFor(ScreenEvents.Activate, target, action);
         AddEventHandlerFor(ScreenEvents.Deactivate, target, action);
         AddEventHandlerFor(ScreenEvents.RequestClose, target, action);
         AddEventHandlerFor(ScreenEvents.Close, target, action);

         if (includeExceptionOccured) {
            AddEventHandlerFor(ScreenEvents.LifecycleExceptionOccured, target, action);
         }
      }

      private void AddEventHandlerFor<TArgs>(
         ScreenEvent<TArgs> @event,
         IScreenBase screen,
         Action<ScreenEvent<TArgs>, TArgs> handlerAction
      ) where TArgs : ScreenEventArgs {
         _sm.Subscribe(b => {
            b.On(@event, screen).Execute(args => handlerAction(@event, args));
         });
      }

      private class TestScreen : DefaultTestScreen { }

      private class Subject { }
   }
}