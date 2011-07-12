namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class EventSubscriptionManager : IEventSubscriptionStore {
      private readonly List<IEventSubscription> _subscriptions;

      public EventSubscriptionManager(
         EventAggregator aggregator,
         Action<SubscriptionBuilderInterface> subscriptionAction
      ) {
         _subscriptions = new List<IEventSubscription>();

         subscriptionAction(new SubscriptionBuilderInterface(_subscriptions));

         IEventSubscriptionRepository repository = aggregator;
         repository.AddSubscriptionStore(this);
      }

      public void RemoveAllSubscriptions() {
         _subscriptions.Clear();
      }

      public void RemoveSubscriptionsTo(IEvent @event) {
         Contract.Requires<ArgumentNullException>(@event != null);
         _subscriptions.RemoveAll(x => x.Event == @event);
      }

      IEnumerable<IEventSubscription> IEventSubscriptionStore.Subscriptions {
         get { return _subscriptions; }
      }
   }
}
