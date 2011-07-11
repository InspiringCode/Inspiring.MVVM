namespace Inspiring.Mvvm.Common {
   using System;
   using System.Diagnostics.Contracts;

   public static class DefaultEventExtensions {
      public static SubscriptionBuilderInterface<TPayload> On<TPayload>(
         this SubscriptionBuilderInterface rootBuilder,
         Event<TPayload> @event
      ) {
         Contract.Requires<ArgumentNullException>(rootBuilder != null);
         Contract.Requires<ArgumentNullException>(@event != null);

         return new SubscriptionBuilderInterface<TPayload>(rootBuilder, @event);
      }
   }
}
