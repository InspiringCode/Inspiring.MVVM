namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;

   public sealed class EventSubscriptionManager : IEventSubscriptionStore {
      private readonly List<IEventSubscription> _subscriptions;

      public EventSubscriptionManager(EventAggregator aggregator) {
         Check.NotNull(aggregator, nameof(aggregator));

         _subscriptions = new List<IEventSubscription>();

         IEventSubscriptionRepository repository = aggregator;
         repository.AddSubscriptionStore(this);
      }

      public EventSubscriptionManager(
         EventAggregator aggregator,
         Action<SubscriptionBuilderInterface> subscriptionAction
      )
         : this(aggregator) {

         Subscribe(subscriptionAction);
      }

      public void Subscribe(Action<SubscriptionBuilderInterface> subscriptionAction) {
         subscriptionAction(new SubscriptionBuilderInterface(_subscriptions));
      }

      public void RemoveAllSubscriptions() {
         _subscriptions.Clear();
      }

      public void RemoveSubscriptionsTo(IEvent @event) {
         Check.NotNull(@event, nameof(@event));
         _subscriptions.RemoveAll(x => x.Event == @event);
      }

      IEnumerable<IEventSubscription> IEventSubscriptionStore.Subscriptions {
         get { return _subscriptions; }
      }
   }
}
