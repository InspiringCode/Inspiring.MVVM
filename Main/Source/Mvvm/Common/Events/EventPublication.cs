namespace Inspiring.Mvvm.Common {
   using System.Diagnostics.Contracts;

   public sealed class EventPublication<TPayload> {
      public EventPublication(IEvent<TPayload> @event, TPayload payload) {
         Contract.Requires(@event != null);
         Contract.Requires(payload != null);

         Event = @event;
         Payload = payload;
      }

      public IEvent<TPayload> Event { get; private set; }
      public TPayload Payload { get; private set; }
   }
}
