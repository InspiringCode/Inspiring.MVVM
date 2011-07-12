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
      public SubscriptionBuilderInterface(EventSubscriptionBuilder<TPayload> builder) {
         Contract.Requires<ArgumentNullException>(builder != null);
         Builder = builder;
      }

      public EventSubscriptionBuilder<TPayload> Builder { get; private set; }

      public void Execute(Action<TPayload> handler, ExecutionOrder order = ExecutionOrder.Default) {
         Contract.Requires<ArgumentNullException>(handler != null);

         Builder.Handler = handler;
         Builder.ExecutionOrder = order;
         Builder.Build();
      }
   }
}
