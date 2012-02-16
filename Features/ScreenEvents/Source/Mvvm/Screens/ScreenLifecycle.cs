namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
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

      public static ScreenLifecycleOperations For(
         EventAggregator aggregator,
         IScreenBase target
      ) {
         return new ScreenLifecycleOperations {
            _aggregator = aggregator,
            _target = target
         };
      }

      public void Initialize() {
         PublishEvent(ScreenEvents.Initialize(), new ScreenEventArgs(_target));
      }

      public void Initialize<TSubject>(TSubject subject) {
         PublishEvent(ScreenEvents.Initialize<TSubject>(), new InitializeEventArgs<TSubject>(_target));
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
         ScreenEventArgs args
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

   public class ScreenLifecycle_ {
      private static readonly Action DoNothingAction = () => { };
      private static readonly Func<EventArgs, bool> AlwaysCondition = (_) => true;

      private readonly IScreenBase _parent;
      private readonly EventSubscriptionManager _sm;

      private readonly List<HandlerRegistration> _handlers = new List<HandlerRegistration>();
      private readonly StateTransitionCollection _transitions = new StateTransitionCollection();
      private readonly List<IEvent> _handledEvents = new List<IEvent>();

      public ScreenLifecycle_(EventAggregator aggregator, IScreenBase parent) {
         _parent = parent;
         _sm = new EventSubscriptionManager(aggregator);

         State = LifecycleState.Created;
         DefineTransitions();
      }

      public LifecycleState State {
         get;
         private set;
      }

      public void RegisterHandler<TArgs>(
         ScreenEvent<TArgs> @event,
         Action<TArgs> handler,
         ExecutionOrder order = ExecutionOrder.Default
      ) where TArgs : ScreenEventArgs {
         var r = new HandlerRegistration(
            @event,
            order,
            args => handler((TArgs)args)
         );

         _handlers.Add(r);

         if (IsInitializeEventWithSubject(@event)) {
            EnsureInitializeWithSubjectTransition(@event);
         }
      }

      private void DefineTransitions() {
         DefineCreatedTransitions();
         DefineInitializedTransitions();
         DefineActivatedTransitions();
         DefineDeactivatedTransitions();
         DefineExceptionOccuredTransitions();
      }

      private void DefineCreatedTransitions() {
         DefineTransition(LifecycleState.Created, ScreenEvents.Initialize(), LifecycleState.Initialized);
      }

      private void DefineInitializedTransitions() {
         DefineTransition(LifecycleState.Initialized, ScreenEvents.Activate, LifecycleState.Activated);
         DefineTransition(LifecycleState.Initialized, ScreenEvents.Close, LifecycleState.Closed);

         DefineTransition(
            LifecycleState.Initialized,
            ScreenEvents.RequestClose,
            LifecycleState.Initialized,
            condition: args => args.IsCloseAllowed
         );

         DefineTransition(
            LifecycleState.Initialized,
            ScreenEvents.LifecycleExceptionOccured,
            LifecycleState.ExceptionOccured,
            transitionAction: InvokeClose
         );
      }

      private void DefineActivatedTransitions() {
         DefineTransition(LifecycleState.Activated, ScreenEvents.Deactivate, LifecycleState.Deactivated);

         DefineTransition(
            LifecycleState.Activated,
            ScreenEvents.RequestClose,
            LifecycleState.Activated,
            condition: args => args.IsCloseAllowed
         );

         DefineTransition(
            LifecycleState.Activated,
            ScreenEvents.LifecycleExceptionOccured,
            LifecycleState.ExceptionOccured,
            transitionAction: InvokeDeactivateAndClose
         );
      }

      private void DefineDeactivatedTransitions() {
         DefineTransition(LifecycleState.Deactivated, ScreenEvents.Activate, LifecycleState.Activated);
         DefineTransition(LifecycleState.Deactivated, ScreenEvents.Close, LifecycleState.Closed);

         DefineTransition(
            LifecycleState.Deactivated,
            ScreenEvents.RequestClose,
            LifecycleState.Deactivated,
            condition: args => args.IsCloseAllowed
         );

         DefineTransition(
            LifecycleState.Deactivated,
            ScreenEvents.LifecycleExceptionOccured,
            LifecycleState.ExceptionOccured,
            transitionAction: InvokeClose
         );
      }

      private void DefineExceptionOccuredTransitions() {
         // ?
      }

      private void InvokeDeactivateAndClose() {
         ExecuteHandlers(ScreenEvents.Deactivate, new ScreenEventArgs(_parent));
         InvokeClose();
      }

      private void InvokeClose() {
         ExecuteHandlers(ScreenEvents.Close, new ScreenEventArgs(_parent));
      }

      private void EnsureInitializeWithSubjectTransition<TArgs>(
         ScreenEvent<TArgs> @event
      ) where TArgs : ScreenEventArgs {
         DefineTransition(LifecycleState.Initialized, @event, LifecycleState.Initialized);
      }

      private void DefineTransition<TArgs>(
         LifecycleState from,
         ScreenEvent<TArgs> trigger,
         LifecycleState to,
         Func<TArgs, bool> condition = null,
         Action transitionAction = null
      ) where TArgs : ScreenEventArgs {
         EnsureEventAggregatorRegistration(trigger);

         transitionAction = transitionAction ?? DoNothingAction;

         Func<EventArgs, bool> untypedCondition = condition != null ?
            (EventArgs args) => condition((TArgs)args) :
            AlwaysCondition;

         var key = new StateTransitionKey(from, trigger);
         var t = new StateTransition(key, to, untypedCondition, transitionAction);
         _transitions.Add(t);
      }

      private void EnsureEventAggregatorRegistration<TArgs>(
         ScreenEvent<TArgs> @event
      ) where TArgs : ScreenEventArgs {
         if (!_handledEvents.Contains(@event)) {
            _sm.Subscribe(b => b
               .On(@event, _parent)
               .Execute(args => HandleEvent(@event, args))
            );

            _handledEvents.Add(@event);
         }
      }

      private void HandleEvent(IEvent @event, EventArgs args) {
         var key = new StateTransitionKey(State, @event);

         if (!_transitions.Contains(key)) {
            throw new InvalidOperationException(); // TODO: Message
         }

         StateTransition transition = _transitions[key];

         try {
            ExecuteHandlers(@event, args);
            transition.TransitionAction();
         } finally {
            State = transition.ToState;
         }
      }

      private void ExecuteHandlers(IEvent @event, EventArgs args) {
         IEnumerable<HandlerRegistration> matching = _handlers
            .Where(h => h.Event == @event)
            .OrderBy(h => h.Order);

         foreach (var registration in matching) {
            registration.Handler(args);
         }
      }

      private static bool IsInitializeEventWithSubject<TArgs>(
         ScreenEvent<TArgs> @event
      ) where TArgs : ScreenEventArgs {
         return TypeService.ClosesGenericType(
            typeof(TArgs),
            typeof(InitializeEventArgs<>)
         );
      }

      private struct StateTransitionKey {
         private readonly LifecycleState _fromState;
         private readonly IEvent _trigger;

         public StateTransitionKey(
            LifecycleState from,
            IEvent trigger
         ) {
            _fromState = from;
            _trigger = trigger;
         }

         public LifecycleState FromState {
            get { return _fromState; }
         }

         public IEvent Trigger {
            get { return _trigger; }
         }
      }

      private class StateTransition {
         public StateTransition(
            StateTransitionKey key,
            LifecycleState to,
            Func<EventArgs, bool> condition,
            Action transitionAction
         ) {
            Key = key;
            ToState = to;
            Condition = condition;
            TransitionAction = transitionAction;
         }

         public StateTransitionKey Key { get; private set; }
         public LifecycleState ToState { get; private set; }
         public Func<EventArgs, bool> Condition { get; private set; }
         public Action TransitionAction { get; private set; }
      }

      private class StateTransitionCollection : KeyedCollection<StateTransitionKey, StateTransition> {
         protected override StateTransitionKey GetKeyForItem(StateTransition item) {
            return item.Key;
         }
      }

      private class HandlerRegistration {
         public HandlerRegistration(IEvent @event, ExecutionOrder order, Action<EventArgs> handler) {
            Event = @event;
            Order = order;
            Handler = handler;
         }

         public IEvent Event { get; private set; }
         public ExecutionOrder Order { get; private set; }
         public Action<EventArgs> Handler { get; private set; }
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
