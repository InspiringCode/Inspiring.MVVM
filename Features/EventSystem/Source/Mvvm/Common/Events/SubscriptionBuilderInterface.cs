namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class SubscriptionBuilderInterface {
      private ICollection<IEventSubscription> _subscriptionStore;

      internal SubscriptionBuilderInterface(ICollection<IEventSubscription> subscriptionStore) {
         Contract.Requires(subscriptionStore != null);
         _subscriptionStore = subscriptionStore;
      }

      public void AddSubscription(IEventSubscription subscription) {
         Contract.Requires<ArgumentNullException>(subscription != null);
         _subscriptionStore.Add(subscription);
      }
   }

   public class SubscriptionBuilderInterface<TPayload> {
      private readonly SubscriptionBuilderInterface _rootBuilder;
      private readonly Event<TPayload> _event;

      public SubscriptionBuilderInterface(SubscriptionBuilderInterface rootBuilder, Event<TPayload> @event) {
         Contract.Requires<ArgumentNullException>(rootBuilder != null);
         Contract.Requires<ArgumentNullException>(@event != null);

         _rootBuilder = rootBuilder;
         _event = @event;
      }

      public void Execute(Action<TPayload> handler) {
         Contract.Requires<ArgumentNullException>(handler != null);

         var s = new EventSubscription<TPayload>(_event, handler, ExecutionOrder.Default);
         _rootBuilder.AddSubscription(s);
      }
   }
}
