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
   }
}
