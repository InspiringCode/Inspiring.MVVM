namespace Inspiring.Mvvm.Common {
   public sealed class EventPublication {
      public EventPublication(IEvent @event, object payload) {
         Check.NotNull(@event, nameof(@event));
         Check.NotNull(payload, nameof(payload));

         Event = @event;
         Payload = payload;
      }

      public IEvent Event { get; private set; }
      public object Payload { get; private set; }

      public override string ToString() {
         return Event.ToString();
      }
   }
}
