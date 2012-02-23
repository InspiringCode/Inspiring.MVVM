namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenLifecycleStateMachineTests {
      [TestMethod]
      public void CreatedState_HandlesEventsCorrectly() {
         ParameterizedTest
            .TestCase(Success(LifecycleState.Created, TriggerEvent.Initialize, LifecycleState.Initialized, ScreenEvents.Initialize()))
            .TestCase(Success(LifecycleState.Created, TriggerEvent.InitializeSubject, LifecycleState.Initialized, ScreenEvents.Initialize(), ScreenEvents.Initialize<TestSubject>()))
            .TestCase(InvalidStateException(LifecycleState.Created, TriggerEvent.Activate))
            .TestCase(InvalidStateException(LifecycleState.Created, TriggerEvent.Deactivate))
            .TestCase(InvalidStateException(LifecycleState.Created, TriggerEvent.RequestClose))
            .TestCase(InvalidStateException(LifecycleState.Created, TriggerEvent.Close))
            .TestCase(InvalidStateException(LifecycleState.Created, TriggerEvent.LifecycleException))
            .Run(ExecuteTestCase);
      }

      private void ExecuteTestCase(EventTestCase test) {
         test.Run();
      }

      private static EventTestCase Success(
         LifecycleState initialState,
         TriggerEvent trigger,
         LifecycleState expectedAfterTransition,
         params IEvent[] expectedHandlerExecutions
      ) {
         return new SuccessCase {
            InitialState = initialState,
            Trigger = trigger,
            ExpectedStateAfterTransition = expectedAfterTransition,
            ExpectedHandlerExecutions = expectedHandlerExecutions
         };
      }

      private static EventTestCase InvalidStateException(
         LifecycleState initialState,
         TriggerEvent trigger
      ) {
         return new InvalidStateCase {
            InitialState = initialState,
            Trigger = trigger
         };
      }

      private class TriggerEvent {
         public static readonly TriggerEvent Initialize =
            new TriggerEvent((a, t) => a.Publish(ScreenEvents.Initialize(), new InitializeEventArgs(t)));

         public static readonly TriggerEvent InitializeSubject =
            new TriggerEvent((a, t) => a.Publish(ScreenEvents.Initialize(), new InitializeEventArgs<TestSubject>(t, new TestSubject())));

         public static readonly TriggerEvent Activate =
            new TriggerEvent((a, t) => a.Publish(ScreenEvents.Activate, new ScreenEventArgs(t)));

         public static readonly TriggerEvent Deactivate =
            new TriggerEvent((a, t) => a.Publish(ScreenEvents.Deactivate, new ScreenEventArgs(t)));

         public static readonly TriggerEvent RequestClose =
            new TriggerEvent((a, t) => a.Publish(ScreenEvents.RequestClose, new RequestCloseEventArgs(t)));

         public static readonly TriggerEvent Close =
            new TriggerEvent((a, t) => a.Publish(ScreenEvents.Close, new ScreenEventArgs(t)));

         public static readonly TriggerEvent LifecycleException =
            new TriggerEvent((a, t) => a.Publish(ScreenEvents.LifecycleExceptionOccured, new ScreenEventArgs(t)));

         private readonly Action<EventAggregator, IScreenBase> _publishAction;

         private TriggerEvent(Action<EventAggregator, IScreenBase> publishAction) {
            _publishAction = publishAction;
         }

         public void PublishEvent(EventAggregator aggregator, IScreenBase target) {
            _publishAction(aggregator, target);
         }
      }

      private class EventTestCase {
         protected EventTestCase() {
            ActuallyHandlerExecutions = new List<IEvent>();
            Aggregator = new EventAggregator();
            Screen = new TestScreen();
            Lifecycle = new ScreenLifecycle_(Aggregator, Screen);
         }

         public TriggerEvent Trigger { get; set; }

         public LifecycleState InitialState { get; set; }

         protected EventAggregator Aggregator { get; set; }

         protected TestScreen Screen { get; set; }

         protected ScreenLifecycle_ Lifecycle { get; set; }

         protected List<IEvent> ActuallyHandlerExecutions { get; set; }

         public virtual void Run() {
         }

         protected void SetLifecycleStateTo(LifecycleState state) {
            var operations = new ScreenLifecycleOperations(Aggregator, Screen);

            if (state == LifecycleState.Created) {
               return;
            }

            operations.Initialize();

            if (state == LifecycleState.Initialized) {
               return;
            }

            operations.Activate();

            if (state == LifecycleState.Activated) {
               return;
            }

            operations.Deactivate();

            if (state == LifecycleState.Deactivated) {
               return;
            }

            operations.Close();

            if (state == LifecycleState.Closed) {
               return;
            }

            throw new NotSupportedException();
         }

         protected void AttachEventHandlers() {
            AttachHandler(ScreenEvents.Initialize());
            AttachHandler(ScreenEvents.Initialize<TestSubject>());
            AttachHandler(ScreenEvents.Activate);
            AttachHandler(ScreenEvents.Deactivate);
            AttachHandler(ScreenEvents.RequestClose);
            AttachHandler(ScreenEvents.Close);
            AttachHandler(ScreenEvents.LifecycleExceptionOccured);
         }

         private void AttachHandler<TArgs>(ScreenEvent<TArgs> @event) where TArgs : ScreenEventArgs {
            Lifecycle.RegisterHandler(@event, args => ActuallyHandlerExecutions.Add(@event));
         }

      }

      private class SuccessCase : EventTestCase {
         public LifecycleState ExpectedStateAfterTransition { get; set; }
         public IEvent[] ExpectedHandlerExecutions { get; set; }

         public override void Run() {
            SetLifecycleStateTo(InitialState);
            AttachEventHandlers();

            Trigger.PublishEvent(Aggregator, Screen);

            Assert.AreEqual(ExpectedStateAfterTransition, Lifecycle.State);

            if (!ExpectedHandlerExecutions.SequenceEqual(ActuallyHandlerExecutions)) {
               Assert.Fail(
                  "Expected handlers '{0}' to be executed but were '{1}'",
                  String.Join(", ", ExpectedHandlerExecutions.AsEnumerable()),
                  String.Join(", ", ActuallyHandlerExecutions)
               );
            }
         }

         public override string ToString() {
            return String.Format(
               "Event {0} in state {1} sould trigger transition to {2}",
               Trigger,
               InitialState,
               ExpectedStateAfterTransition
            );
         }
      }

      private class InvalidStateCase : EventTestCase {

         public override void Run() {
            //AssertHelper.Throws<InvalidOperationException>(() => 

            //);
         }

         public override string ToString() {
            return String.Format(
               "Event {0} in state {1} sould throw exception",
               Trigger,
               InitialState
            );
         }
      }

      private class TestScreen : DefaultTestScreen { }

      private class TestSubject { }
   }
}