namespace Inspiring.Mvvm.Common {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;

   public class EventSubscription<TPayload> : IEventSubscription<TPayload> {
      private readonly IEvent<TPayload> _event;
      private readonly Action<TPayload> _handler;
      private readonly ExecutionOrder _executionOrder;
      private readonly IEventCondition<TPayload>[] _conditions;

      public EventSubscription(
         IEvent<TPayload> @event,
         Action<TPayload> handler,
         ExecutionOrder executionOrder,
         IEventCondition<TPayload>[] conditions = null
      ) {
         Contract.Requires<ArgumentNullException>(@event != null);
         Contract.Requires<ArgumentNullException>(handler != null);

         _event = @event;
         _handler = handler;
         _executionOrder = executionOrder;
         _conditions = conditions;
      }

      public IEvent Event {
         get { return _event; }
      }

      public ExecutionOrder ExecutionOrder {
         get { return _executionOrder; }
      }

      public bool Matches(EventPublication<TPayload> publication) {
         return
            publication.Event == _event &&
            ConditionsMatch(publication.Payload);
      }

      public void Invoke(TPayload payload) {
         _handler(payload);
      }

      private bool ConditionsMatch(TPayload payload) {
         return
            _conditions == null ||
            _conditions.All(x => x.IsTrue(payload));
      }
   }
}
