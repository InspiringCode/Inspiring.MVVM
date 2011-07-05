namespace Inspiring.Mvvm.Common {
   public interface IEvent {
   }

   public interface IEvent<TPayload> : IEvent {
      void Raise(IEventSubscriptionRepository allSubscriptions, TPayload payload);
   }
}
