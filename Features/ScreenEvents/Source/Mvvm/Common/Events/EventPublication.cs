namespace Inspiring.Mvvm.Common {
   using System.Diagnostics.Contracts;

   public sealed class EventPublication {
      public EventPublication(IEvent @event, object payload) {
         Contract.Requires(@event != null);
         Contract.Requires(payload != null);

         Event = @event;
         Payload = payload;
      }

      public IEvent Event { get; private set; }
      public object Payload { get; private set; }
   }
}
