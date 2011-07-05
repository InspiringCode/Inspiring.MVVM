namespace Inspiring.Mvvm.Common {
   using System.Collections.Generic;

   public interface IEventSubscriptionRepository {
      //IEnumerable<TSubscription> GetSubscriptionsFor<TSubscription, TPayload>(
      //   EventOccurrence<TPayload> eventOccurrence
      //);

      //IEnumerable<TSubscription> GetSubscriptions<TSubscription>();

      void AddSubscriptionStore(IEnumerable<IEventSubscription> store);
   }
}
