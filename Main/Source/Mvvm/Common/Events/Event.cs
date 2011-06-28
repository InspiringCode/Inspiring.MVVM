namespace Inspiring.Mvvm.Common {

   public class Event<TPayload> : IEvent<TPayload> {

      void IEvent<TPayload>.Raise(IEventSubscriptionRepository allSubscriptions, TPayload payload) {

      }
   }
}
