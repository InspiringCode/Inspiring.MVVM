namespace Inspiring.Mvvm.Common {
   using System.Collections.Generic;

   public interface IEventSubscriptionStore {
      IEnumerable<IEventSubscription> Subscriptions { get; }
   }
}
