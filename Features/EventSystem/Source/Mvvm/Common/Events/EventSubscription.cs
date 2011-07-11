namespace Inspiring.Mvvm.Common {
   using System;
   using System.Diagnostics.Contracts;

   public class EventSubscription<TPayload> : IEventSubscription<TPayload> {
      private readonly IEvent<TPayload> _event;
      private readonly Action<TPayload> _handler;
      private readonly ExecutionOrder _executionOrder;

      public EventSubscription(
         IEvent<TPayload> @event,
         Action<TPayload>
         handler, ExecutionOrder executionOrder
      ) {
         Contract.Requires<ArgumentNullException>(@event != null);
         Contract.Requires<ArgumentNullException>(handler != null);

         _event = @event;
         _handler = handler;
         _executionOrder = executionOrder;
      }

      public IEvent Event {
         get { return _event; }
      }

      public ExecutionOrder ExecutionOrder {
         get { return _executionOrder; }
      }

      public bool Matches(EventPublication<TPayload> publication) {
         return publication.Event == _event;
      }

      public void Invoke(TPayload payload) {
         _handler(payload);
      }
   }
}
