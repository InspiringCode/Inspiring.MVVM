using System.Collections.Generic;
namespace Inspiring.Mvvm.Common {

   public interface IEventSubscriptionRepository {
      IEnumerable<IEventSubscription> GetSubscriptions(
         EventPublication publication
      );

      void AddSubscriptionStore(IEventSubscriptionStore store);
   }
}
