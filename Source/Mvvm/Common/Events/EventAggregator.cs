namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   public sealed class EventAggregator : IEventSubscriptionRepository {

      private WeakCollection<IEventSubscriptionStore> _subscriptionStores =
         new WeakCollection<IEventSubscriptionStore>();

      public void Publish<TPayload>(IEvent<TPayload> @event, TPayload payload) {
         Contract.Requires<ArgumentNullException>(@event != null);
         Contract.Requires<ArgumentNullException>(payload != null);

         @event.Publish(this, payload);
      }

      void IEventSubscriptionRepository.AddSubscriptionStore(IEventSubscriptionStore store) {
         _subscriptionStores.Add(store);
      }

      IEnumerable<IEventSubscription<TPayload>> IEventSubscriptionRepository.GetSubscriptions<TPayload>(
         EventPublication<TPayload> publication
      ) {
         return _subscriptionStores
            .SelectMany(x => x.Subscriptions)
            .OfType<IEventSubscription<TPayload>>()
            .Where(x => x.Matches(publication))
            .ToArray();
      }
   }
}
