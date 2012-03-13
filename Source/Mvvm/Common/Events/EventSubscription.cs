namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   public class EventSubscription<TPayload> : IEventSubscription {
      public static readonly IEvent<TPayload> AnyEvent = null;

      private static readonly IEnumerable<IEventCondition> NoConditions = Enumerable.Empty<IEventCondition>();
      private readonly IEvent<TPayload> _event;
      private readonly Action<TPayload> _handler;
      private readonly ExecutionOrder _executionOrder;
      private readonly IEnumerable<IEventCondition> _conditions;

      public EventSubscription(
         IEvent<TPayload> @event,
         Action<TPayload> handler,
         ExecutionOrder executionOrder,
         IEventCondition[] conditions = null
      ) {
         Contract.Requires<ArgumentNullException>(handler != null);

         _event = @event;
         _handler = handler;
         _executionOrder = executionOrder;
         _conditions = conditions ?? NoConditions;
      }

      public IEvent Event {
         get { return _event; }
      }

      public ExecutionOrder ExecutionOrder {
         get { return _executionOrder; }
      }

      public bool Matches(EventPublication publication) {
         return
            (Event == AnyEvent || publication.Event == _event) &&
            _conditions.All(x => x.IsTrue(publication.Payload));
      }

      public void Invoke(EventPublication publication) {
         _handler((TPayload)publication.Payload);
      }
   }
}
