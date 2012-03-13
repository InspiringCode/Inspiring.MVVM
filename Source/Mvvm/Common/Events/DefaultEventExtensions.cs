namespace Inspiring.Mvvm.Common {
   using System;
   using System.Diagnostics.Contracts;

   public static class DefaultEventExtensions {
      public static SubscriptionBuilderInterface<TPayload> On<TPayload>(
         this SubscriptionBuilderInterface root,
         Event<TPayload> @event
      ) {
         Contract.Requires<ArgumentNullException>(root != null);
         Contract.Requires<ArgumentNullException>(@event != null);

         var builder = new EventSubscriptionBuilder<TPayload>(root) {
            Event = @event
         };

         return new SubscriptionBuilderInterface<TPayload>(builder);
      }

      public static SubscriptionBuilderInterface<TArgs> On<TTarget, TArgs>(
         this SubscriptionBuilderInterface root,
         HierarchicalEvent<TTarget, TArgs> @event,
         TTarget target
      ) where TArgs : HierarchicalEventArgs<TTarget> {
         Contract.Requires<ArgumentNullException>(root != null);
         Contract.Requires<ArgumentNullException>(@event != null);

         var builder = new HierarchicalEventSubscriptionBuilder<TTarget, TArgs>(root, target) {
            Event = @event
         };

         return new SubscriptionBuilderInterface<TArgs>(builder);
      }

      public static SubscriptionBuilderInterface<TPayload> When<TPayload>(
         this SubscriptionBuilderInterface<TPayload> builder,
         Func<TPayload, bool> condition
      ) {
         builder
            .Builder
            .Conditions
            .Add(new DelegateEventCondition<TPayload>(condition));

         return builder;
      }

      private class HierarchicalEventSubscriptionBuilder<TTarget, TArgs> : EventSubscriptionBuilder<TArgs> {
         public HierarchicalEventSubscriptionBuilder(
            SubscriptionBuilderInterface builderInterface,
            TTarget target
         )
            : base(builderInterface) {

            Target = target;
         }

         public TTarget Target { get; private set; }

         protected override IEventSubscription CreateSubscription(IEventCondition[] conditions) {
            return new HierarchicalEventSubscription<TTarget, TArgs>(
               Event,
               Handler,
               ExecutionOrder,
               conditions,
               Target
            );
         }
      }
   }
}
