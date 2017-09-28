namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;

   public sealed class SubscriptionBuilderInterface {
      private ICollection<IEventSubscription> _subscriptionStore;

      internal SubscriptionBuilderInterface(ICollection<IEventSubscription> subscriptionStore) {
         Check.NotNull(subscriptionStore, nameof(subscriptionStore));
         _subscriptionStore = subscriptionStore;
      }

      public void AddSubscription(IEventSubscription subscription) {
         Check.NotNull(subscription, nameof(subscription));
         _subscriptionStore.Add(subscription);
      }
   }

   public class SubscriptionBuilderInterface<TPayload> {
      public SubscriptionBuilderInterface(EventSubscriptionBuilder<TPayload> builder) {
         Check.NotNull(builder, nameof(builder));
         Builder = builder;
      }

      public EventSubscriptionBuilder<TPayload> Builder { get; private set; }

      public void Execute(Action<TPayload> handler, ExecutionOrder order = ExecutionOrder.Default) {
         Check.NotNull(handler, nameof(handler));

         Builder.Handler = handler;
         Builder.ExecutionOrder = order;
         Builder.Build();
      }
   }
}
