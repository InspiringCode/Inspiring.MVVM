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

      [TestMethod]
      public void InitializedState_HandlesEventsCorrectly() {
         ParameterizedTest
            .TestCase(Success(LifecycleState.Initialized, TriggerEvent.Initialize, LifecycleState.Initialized))
            .TestCase(Success(LifecycleState.Initialized, TriggerEvent.InitializeSubject, LifecycleState.Initialized))
            .TestCase(Success(LifecycleState.Initialized, TriggerEvent.Activate, LifecycleState.Activated, ScreenEvents.Activate))
            .TestCase(Success(LifecycleState.Initialized, TriggerEvent.Close, LifecycleState.Closed, ScreenEvents.Close))
            .TestCase(Success(LifecycleState.Initialized, TriggerEvent.RequestClose, LifecycleState.Initialized, ScreenEvents.RequestClose))
            .TestCase(Success(LifecycleState.Initialized, TriggerEvent.LifecycleException, LifecycleState.ExceptionOccured, ScreenEvents.Close))
            .TestCase(InvalidStateException(LifecycleState.Initialized, TriggerEvent.Deactivate))
            .Run(ExecuteTestCase);
      }

      [TestMethod]
      public void ActivatedState_HandlesEventsCorrectly() {
         ParameterizedTest
            .TestCase(Success(LifecycleState.Activated, TriggerEvent.Deactivate, LifecycleState.Deactivated, ScreenEvents.Deactivate))
            .TestCase(Success(LifecycleState.Activated, TriggerEvent.RequestClose, LifecycleState.Activated, ScreenEvents.RequestClose))
            .TestCase(Success(LifecycleState.Activated, TriggerEvent.LifecycleException, LifecycleState.ExceptionOccured, ScreenEvents.Deactivate, ScreenEvents.Close))
            .TestCase(InvalidStateException(LifecycleState.Activated, TriggerEvent.Initialize))
            .TestCase(InvalidStateException(LifecycleState.Activated, TriggerEvent.InitializeSubject))
            .TestCase(InvalidStateException(LifecycleState.Activated, TriggerEvent.Activate))
            .TestCase(InvalidStateException(LifecycleState.Activated, TriggerEvent.Close))
            .Run(ExecuteTestCase);
      }

      [TestMethod]
      public void DeactivatedState_HandlesEventsCorrectly() {
         ParameterizedTest
            .TestCase(Success(LifecycleState.Deactivated, TriggerEvent.Activate, LifecycleState.Activated, ScreenEvents.Activate))
            .TestCase(Success(LifecycleState.Deactivated, TriggerEvent.Close, LifecycleState.Closed, ScreenEvents.Close))
            .TestCase(Success(LifecycleState.Deactivated, TriggerEvent.RequestClose, LifecycleState.Deactivated, ScreenEvents.RequestClose))
            .TestCase(Success(LifecycleState.Deactivated, TriggerEvent.LifecycleException, LifecycleState.ExceptionOccured, ScreenEvents.Close))
            .TestCase(InvalidStateException(LifecycleState.Deactivated, TriggerEvent.Initialize))
            .TestCase(InvalidStateException(LifecycleState.Deactivated, TriggerEvent.InitializeSubject))
            .TestCase(InvalidStateException(LifecycleState.Deactivated, TriggerEvent.Deactivate))
            .Run(ExecuteTestCase);
      }

      [TestMethod]
      public void ClosedState_HandlesEventsCorrectly() {
         ParameterizedTest
            .TestCase(Success(LifecycleState.Closed, TriggerEvent.LifecycleException, LifecycleState.ExceptionOccured))
            .TestCase(InvalidStateException(LifecycleState.Closed, TriggerEvent.Initialize))
            .TestCase(InvalidStateException(LifecycleState.Closed, TriggerEvent.InitializeSubject))
            .TestCase(InvalidStateException(LifecycleState.Closed, TriggerEvent.Activate))
            .TestCase(InvalidStateException(LifecycleState.Closed, TriggerEvent.Deactivate))
            .TestCase(InvalidStateException(LifecycleState.Closed, TriggerEvent.RequestClose))
            .TestCase(InvalidStateException(LifecycleState.Closed, TriggerEvent.Close))
            .Run(ExecuteTestCase);
      }

      [TestMethod]
      public void ExceptionState_HandlesEventsCorrectly() {
         ParameterizedTest
            .TestCase(InvalidStateException(LifecycleState.ExceptionOccured, TriggerEvent.LifecycleException))
            .TestCase(InvalidStateException(LifecycleState.ExceptionOccured, TriggerEvent.Initialize))
            .TestCase(InvalidStateException(LifecycleState.ExceptionOccured, TriggerEvent.InitializeSubject))
            .TestCase(InvalidStateException(LifecycleState.ExceptionOccured, TriggerEvent.Activate))
            .TestCase(InvalidStateException(LifecycleState.ExceptionOccured, TriggerEvent.Deactivate))
            .TestCase(InvalidStateException(LifecycleState.ExceptionOccured, TriggerEvent.RequestClose))
            .TestCase(InvalidStateException(LifecycleState.ExceptionOccured, TriggerEvent.Close))
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

      private abstract class TriggerEvent {
         public static readonly TriggerEvent Initialize =
            Create(ScreenEvents.Initialize(), t => new InitializeEventArgs(t));

         public static readonly TriggerEvent InitializeSubject =
            Create(ScreenEvents.Initialize(), t => new InitializeEventArgs<TestSubject>(t, new TestSubject()));

         public static readonly TriggerEvent Activate =
            Create(ScreenEvents.Activate, t => new ScreenEventArgs(t));

         public static readonly TriggerEvent Deactivate =
            Create(ScreenEvents.Deactivate, t => new ScreenEventArgs(t));

         public static readonly TriggerEvent RequestClose =
            Create(ScreenEvents.RequestClose, t => new RequestCloseEventArgs(t));

         public static readonly TriggerEvent Close =
            Create(ScreenEvents.Close, t => new ScreenEventArgs(t));

         public static readonly TriggerEvent LifecycleException =
            Create(ScreenEvents.LifecycleExceptionOccured, t => new ScreenEventArgs(t));

         public abstract void PublishEvent(EventAggregator aggregator, IScreenBase target);

         private static TriggerEvent Create<TArgs>(
            ScreenEvent<TArgs> @event,
            Func<IScreenBase, TArgs> argsFactory
         ) where TArgs : ScreenEventArgs {
            return new GenericTriggerEvent<TArgs>(@event, argsFactory);
         }

         private class GenericTriggerEvent<TArgs> :
            TriggerEvent
            where TArgs : ScreenEventArgs {

            private readonly ScreenEvent<TArgs> _event;
            private readonly Func<IScreenBase, TArgs> _argsFactory;

            public GenericTriggerEvent(ScreenEvent<TArgs> @event, Func<IScreenBase, TArgs> argsFactory) {
               _event = @event;
               _argsFactory = argsFactory;
            }

            public override void PublishEvent(EventAggregator aggregator, IScreenBase target) {
               aggregator.Publish(_event, _argsFactory(target));
            }

            public override string ToString() {
               return _event.ToString();
            }
         }
      }

      private class EventTestCase {
         protected EventTestCase() {
            ActuallyHandlerExecutions = new List<IEvent>();
            Aggregator = new EventAggregator();
            Screen = new TestScreen();
            Lifecycle = new ScreenLifecycle(Aggregator, Screen);
         }

         public TriggerEvent Trigger { get; set; }

         public LifecycleState InitialState { get; set; }

         protected EventAggregator Aggregator { get; set; }

         protected TestScreen Screen { get; set; }

         protected ScreenLifecycle Lifecycle { get; set; }

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

            if (state == LifecycleState.ExceptionOccured) {
               Aggregator.Publish(
                  ScreenEvents.LifecycleExceptionOccured,
                  new ScreenEventArgs(Screen)
               );
            } else {
               throw new NotSupportedException();
            }
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
            SetLifecycleStateTo(InitialState);

            AssertHelper.Throws<InvalidOperationException>(() =>
               Trigger.PublishEvent(Aggregator, Screen)
            );
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