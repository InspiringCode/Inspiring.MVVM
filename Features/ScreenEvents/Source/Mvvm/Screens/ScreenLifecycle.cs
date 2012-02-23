namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   public enum LifecycleState {
      Created,
      Initialized,
      Activated,
      Deactivated,
      Closed,
      ExceptionOccured
   }

   internal class ScreenLifecycleOperations {
      private EventAggregator _aggregator;
      private IScreenBase _target;

      public ScreenLifecycleOperations(
         EventAggregator aggregator,
         IScreenBase target
      ) {
         _aggregator = aggregator;
         _target = target;
      }
      
      public void Initialize() {
         PublishEvent(ScreenEvents.Initialize(), new InitializeEventArgs(_target));
      }

      public void Initialize<TSubject>(TSubject subject) {
         PublishEvent(
            ScreenEvents.Initialize<TSubject>(),
            new InitializeEventArgs<TSubject>(_target, subject)
         );
      }

      public void Activate() {
         PublishEvent(ScreenEvents.Activate, new ScreenEventArgs(_target));
      }

      public void Deactivate() {
         PublishEvent(ScreenEvents.Deactivate, new ScreenEventArgs(_target));
      }

      public bool RequestClose() {


         throw new NotImplementedException();

      }

      public void Close() {
         throw new NotImplementedException();
      }

      private void PublishEvent<TArgs>(
         ScreenEvent<TArgs> @event,
         TArgs args
      ) where TArgs : ScreenEventArgs {
         try {
            _aggregator.Publish(@event, args);
         } catch (Exception ex) {
            if (ex.IsCritical()) {
               throw;
            }

            _aggregator.Publish(
               ScreenEvents.LifecycleExceptionOccured,
               new ScreenEventArgs(_target)
            );
         }
      }
   }

   public partial class ScreenLifecycle_ {
      private static readonly Func<EventPublication, bool> AlwaysTrueCondition = (_) => true;
      private static readonly ScreenEvent<ScreenEventArgs> AnyEvent = null;

      private readonly IScreenBase _parent;
      private readonly EventSubscriptionManager _subscriptionManager;
      private readonly LifecycleStateMachine _sm;
      private readonly List<IEventSubscription> _handlers = new List<IEventSubscription>();

      public ScreenLifecycle_(EventAggregator aggregator, IScreenBase parent) {
         _parent = parent;
         _subscriptionManager = new EventSubscriptionManager(aggregator);
         _sm = new LifecycleStateMachine(_parent);

         DefineTransitions();

         _subscriptionManager.Subscribe(b =>
            b.AddSubscription(_sm)
         );
      }

      public LifecycleState State {
         get { return _sm.State; }
      }

      public void RegisterHandler<TArgs>(
         ScreenEvent<TArgs> @event,
         Action<TArgs> handler,
         ExecutionOrder order = ExecutionOrder.Default
      ) where TArgs : ScreenEventArgs {
         IEventSubscription sub;
         bool isInitializeWithoutSubject = typeof(TArgs) == typeof(InitializeEventArgs);
         bool isInitializeWithSubject = typeof(InitializeEventArgs).IsAssignableFrom(typeof(TArgs));

         if (isInitializeWithoutSubject) {
            sub = new InitializeSubscription(
               @event,
               order,
               (Action<InitializeEventArgs>)handler
            );
         } else if (isInitializeWithSubject) {
            Type subjectType = typeof(TArgs)
               .GetGenericArguments()
               .Single();

            Type subscriptionType = typeof(InitializeSubscription<>)
               .MakeGenericType(subjectType);

            sub = (IEventSubscription)Activator.CreateInstance(
               subscriptionType,
               @event,
               order,
               handler
            );
         } else {
            sub = new EventSubscription<TArgs>(
               @event,
               handler,
               order
            );
         }

         _handlers.Add(sub);
      }

      private void DefineTransitions() {
         DefineCreatedTransitions();
         DefineInitializedTransitions();
         DefineActivatedTransitions();
         DefineDeactivatedTransitions();
         DefineExceptionOccuredTransitions();
      }

      private void DefineCreatedTransitions() {
         DefineTransition(
            LifecycleState.Created,
            LifecycleState.Initialized,
            AnyEvent,
            ExecuteHandlers,
            condition: pub => pub.Payload is InitializeEventArgs
         );
      }

      private void DefineInitializedTransitions() {
         DefineTransition(
            LifecycleState.Initialized,
            LifecycleState.Activated,
            ScreenEvents.Activate,
            ExecuteHandlers
         );

         DefineTransition(
            LifecycleState.Initialized,
            LifecycleState.Closed,
            ScreenEvents.Close,
            ExecuteHandlers
         );

         DefineTransition(
            LifecycleState.Initialized,
            LifecycleState.Initialized,
            ScreenEvents.RequestClose,
            ExecuteHandlers,
            condition: CloseWasNotCancelled
         );

         DefineTransition(
            LifecycleState.Initialized,
            LifecycleState.ExceptionOccured,
            ScreenEvents.LifecycleExceptionOccured,
            InvokeClose
         );
      }

      private void DefineActivatedTransitions() {
         DefineTransition(
            LifecycleState.Activated,
            LifecycleState.Deactivated,
            ScreenEvents.Deactivate,
            ExecuteHandlers
         );

         DefineTransition(
            LifecycleState.Activated,
            LifecycleState.Activated,
            ScreenEvents.RequestClose,
            ExecuteHandlers,
            condition: CloseWasNotCancelled
         );

         DefineTransition(
            LifecycleState.Activated,
            LifecycleState.ExceptionOccured,
            ScreenEvents.LifecycleExceptionOccured,
            InvokeDeactivateAndClose
         );
      }

      private void DefineDeactivatedTransitions() {
         DefineTransition(
            LifecycleState.Deactivated,
            LifecycleState.Activated,
            ScreenEvents.Activate,
            ExecuteHandlers
         );

         DefineTransition(
            LifecycleState.Deactivated,
            LifecycleState.Closed,
            ScreenEvents.Close,
            ExecuteHandlers
         );

         DefineTransition(
            LifecycleState.Deactivated,
            LifecycleState.Deactivated,
            ScreenEvents.RequestClose,
            ExecuteHandlers,
            condition: CloseWasNotCancelled
         );

         DefineTransition(
            LifecycleState.Deactivated,
            LifecycleState.ExceptionOccured,
            ScreenEvents.LifecycleExceptionOccured,
            InvokeClose
         );
      }

      private void DefineExceptionOccuredTransitions() {
         // ?
      }

      private void DefineTransition<TArgs>(
         LifecycleState from,
         LifecycleState to,
         ScreenEvent<TArgs> on,
         Action<EventPublication> action,
         Func<EventPublication, bool> condition = null
      ) where TArgs : ScreenEventArgs {
         condition = condition ?? AlwaysTrueCondition;

         if (on != null) {
            condition = pub =>
               pub.Event == on && condition(pub);
         }

         _sm.DefineTransition(from, to, condition, action);
      }

      private void ExecuteHandlers(EventPublication publication) {
         IEnumerable<IEventSubscription> matching = _handlers
            .Where(s => s.Matches(publication))
            .OrderBy(h => h.ExecutionOrder);

         foreach (var registration in matching) {
            registration.Invoke(publication);
         }
      }

      private void InvokeDeactivateAndClose(EventPublication originalPublication) {
         var pub = new EventPublication(ScreenEvents.Deactivate, new ScreenEventArgs(_parent));
         ExecuteHandlers(pub);
         InvokeClose(originalPublication);
      }

      private void InvokeClose(EventPublication originalPublication) {
         var pub = new EventPublication(ScreenEvents.Close, new ScreenEventArgs(_parent));
         ExecuteHandlers(pub);
      }

      private static bool CloseWasNotCancelled(EventPublication publication) {
         Contract.Requires(publication.Event == ScreenEvents.RequestClose);

         RequestCloseEventArgs args = (RequestCloseEventArgs)publication.Payload;
         return args.IsCloseAllowed;
      }

      private class LifecycleStateMachine :
         SimpleStateMachine<EventPublication, LifecycleState>,
         IHierarchicalEventSubscription<IScreenBase> {

         private readonly IScreenBase _target;

         public LifecycleStateMachine(IScreenBase parent) {
            _target = parent;
         }

         public IEvent Event {
            get { return null; }
         }

         public ExecutionOrder ExecutionOrder {
            get { return ExecutionOrder.Default; }
         }

         public bool Matches(EventPublication publication) {
            return true;
         }

         public void Invoke(EventPublication publication) {
            HandleEvent(publication);
         }

         public IScreenBase Target {
            get { return _target; }
         }
      }

      private abstract class AbstractInitializeSubscription : IEventSubscription {
         private readonly IEvent _event;
         private readonly ExecutionOrder _executionOrder;

         public AbstractInitializeSubscription(
            IEvent @event,
            ExecutionOrder executionOrder
         ) {
            _event = @event;
            _executionOrder = executionOrder;
         }

         public IEvent Event {
            get { return _event; }
         }

         public ExecutionOrder ExecutionOrder {
            get { return _executionOrder; }
         }

         public bool Matches(EventPublication publication) {
            return publication.Payload is InitializeEventArgs;
         }

         public void Invoke(EventPublication publication) {
            TryInvokeCore((InitializeEventArgs)publication.Payload);
         }

         public abstract void TryInvokeCore(InitializeEventArgs args);
      }

      private class InitializeSubscription : AbstractInitializeSubscription {
         private readonly Action<InitializeEventArgs> _handler;

         public InitializeSubscription(
            IEvent @event,
            ExecutionOrder executionOrder,
            Action<InitializeEventArgs> handler
         )
            : base(@event, executionOrder) {
            _handler = handler;
         }

         public override void TryInvokeCore(InitializeEventArgs args) {
            _handler(args);
         }
      }

      private class InitializeSubscription<TSubject> : AbstractInitializeSubscription {
         private readonly Action<InitializeEventArgs<TSubject>> _handler;

         public InitializeSubscription(
            IEvent @event,
            ExecutionOrder executionOrder,
            Action<InitializeEventArgs<TSubject>> handler
         )
            : base(@event, executionOrder) {
            _handler = handler;
         }

         public override void TryInvokeCore(InitializeEventArgs args) {
            if (args.CanConvertTo<TSubject>()) {
               InitializeEventArgs<TSubject> subjectArgs = args.ConvertTo<TSubject>();
               _handler(subjectArgs);
            }
         }
      }
   }

   public partial class ScreenLifecycle_ {
      private class SimpleStateMachine<TEvent, TState> {
         private readonly List<StateTransition> _transitions = new List<StateTransition>();

         public TState State {
            get;
            private set;
         }

         public void DefineTransition(
            TState fromState,
            TState toState,
            Func<TEvent, bool> condition,
            Action<TEvent> action
         ) {
            var t = new StateTransition(fromState, toState, condition, action);
            _transitions.Add(t);
         }

         public void HandleEvent(TEvent @event) {
            var transition = _transitions
               .SingleOrDefault(t =>
                  Object.Equals(t.FromState, State) &&
                  t.Condition(@event)
               );

            if (transition == null) {
               throw new InvalidOperationException(); // TODO: Message
            }

            State = transition.ToState;
            transition.Action(@event);
         }

         private class StateTransition {
            public StateTransition(
               TState fromState,
               TState toState,
               Func<TEvent, bool> condition,
               Action<TEvent> action
            ) {
               FromState = fromState;
               ToState = toState;
               Condition = condition;
               Action = action;
            }

            public TState FromState { get; private set; }
            public TState ToState { get; private set; }
            public Func<TEvent, bool> Condition { get; private set; }
            public Action<TEvent> Action { get; private set; }
         }
      }
   }

   public abstract class ScreenLifecycle : IScreenLifecycle {
      public virtual IScreenLifecycle Parent { get; set; }

      public virtual void Activate() {
      }

      public virtual void Deactivate() {
      }

      public virtual bool RequestClose() {
         return true;
      }

      public virtual void Close() {
      }

      public void Corrupt(object data = null) {
      }
   }
}
