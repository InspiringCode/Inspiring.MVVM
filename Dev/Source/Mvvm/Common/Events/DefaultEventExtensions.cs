namespace Inspiring.Mvvm.Common {
   using System;
   using System.Diagnostics.Contracts;

   public static class DefaultEventExtensions {
      public static EventSubscriptionBuilder<TPayload> On<TPayload>(
         this EventSubscriptionBuilder rootBuilder,
         Event<TPayload> @event
      ) {
         Contract.Requires<ArgumentNullException>(rootBuilder != null);
         Contract.Requires<ArgumentNullException>(@event != null);

         return new EventSubscriptionBuilder<TPayload>(rootBuilder, @event);
      }
   }
}
