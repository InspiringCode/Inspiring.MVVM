namespace Inspiring.Mvvm.Common {
   using System.Collections.Generic;
   using System.Linq;

   public sealed class EventAggregator : IEventSubscriptionRepository {

      private WeakCollection<IEventSubscriptionStore> _subscriptionStores =
         new WeakCollection<IEventSubscriptionStore>();

      public void Publish<TPayload>(IEvent<TPayload> @event, TPayload payload) {
         Check.NotNull(@event, nameof(@event));
         Check.NotNull(payload, nameof(payload));

         @event.Publish(this, payload);
      }

      void IEventSubscriptionRepository.AddSubscriptionStore(IEventSubscriptionStore store) {
         _subscriptionStores.Add(store);
      }

      IEnumerable<IEventSubscription> IEventSubscriptionRepository.GetSubscriptions(
         EventPublication publication
      ) {
         return _subscriptionStores
            .SelectMany(x => x.Subscriptions)
            .Where(x => x.Matches(publication))
            .ToArray();
      }
   }
}
