namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class EventSubscriptionBuilder {
      private ICollection<IEventSubscription> _subscriptionStore;

      internal EventSubscriptionBuilder(ICollection<IEventSubscription> subscriptionStore) {
         Contract.Requires(subscriptionStore != null);
         _subscriptionStore = subscriptionStore;
      }

      public void AddSubscription(IEventSubscription subscription) {
         Contract.Requires<ArgumentNullException>(subscription != null);
         _subscriptionStore.Add(subscription);
      }
   }

   public class EventSubscriptionBuilder<TPayload> {
      private readonly EventSubscriptionBuilder _rootBuilder;
      private readonly Event<TPayload> _event;

      public EventSubscriptionBuilder(EventSubscriptionBuilder rootBuilder, Event<TPayload> @event) {
         Contract.Requires<ArgumentNullException>(rootBuilder != null);
         Contract.Requires<ArgumentNullException>(@event != null);

         _rootBuilder = rootBuilder;
         _event = @event;
      }

      public void Execute(Action<TPayload> handler) {
         Contract.Requires<ArgumentNullException>(handler != null);

         var s = new EventSubscription<TPayload>(_event, handler);
         _rootBuilder.AddSubscription(s);
      }
   }
}
