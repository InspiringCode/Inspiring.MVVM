using System.Collections.Generic;
namespace Inspiring.Mvvm.Common {

   public interface IEventSubscriptionRepository {
      IEnumerable<IEventSubscription<TPayload>> GetSubscriptions<TPayload>(
         EventPublication<TPayload> publication
      );

      void AddSubscriptionStore(IEventSubscriptionStore store);
   }
}
