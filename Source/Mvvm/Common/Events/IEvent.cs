namespace Inspiring.Mvvm.Common {
   public interface IEvent {
   }

   public interface IEvent<TPayload> : IEvent {
      void Publish(IEventSubscriptionRepository allSubscriptions, TPayload payload);
   }
}
