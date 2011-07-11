namespace Inspiring.Mvvm.Common {
   using System;
   using System.Diagnostics.Contracts;

   public class EventSubscription<TPayload> : IEventSubscription<TPayload> {
      private readonly Event<TPayload> _event;
      private readonly Action<TPayload> _handler;

      public EventSubscription(Event<TPayload> @event, Action<TPayload> handler) {
         Contract.Requires<ArgumentNullException>(@event != null);
         Contract.Requires<ArgumentNullException>(handler != null);

         _event = @event;
         _handler = handler;
      }

      IEvent IEventSubscription.Event {
         get { return _event; }
      }

      bool IEventSubscription<TPayload>.Matches(EventPublication<TPayload> publication) {
         return Matches(publication);
      }

      void IEventSubscription<TPayload>.Invoke(TPayload payload) {
         _handler(payload);
      }

      protected bool Matches(EventPublication<TPayload> publication) {
         return publication.Event == _event;
      }
   }
}
