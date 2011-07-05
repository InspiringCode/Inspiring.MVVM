namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class EventSubscriptionManager {
      private readonly List<IEventSubscription> _subscriptions;

      public EventSubscriptionManager(
         EventAggregator aggregator,
         Action<EventSubscriptionBuilder> subscriptionAction
      ) {
         _subscriptions = new List<IEventSubscription>();

         subscriptionAction(new EventSubscriptionBuilder(_subscriptions));

         IEventSubscriptionRepository repository = aggregator;
         repository.AddSubscriptionStore(_subscriptions);
      }

      public void RemoveAllSubscriptions() {
         _subscriptions.Clear();
      }

      public void RemoveSubscriptionsTo(IEvent @event) {
         Contract.Requires<ArgumentNullException>(@event != null);
         _subscriptions.RemoveAll(x => x.Event == @event);
      }
   }
}
