namespace Inspiring.Mvvm.Common {
   using System.Collections.Generic;
   using System.Linq;

   public class Event<TPayload> : IEvent<TPayload> {
      void IEvent<TPayload>.Publish(IEventSubscriptionRepository allSubscriptions, TPayload payload) {
         var publication = new EventPublication(this, payload);

         IEnumerable<IEventSubscription> matching = allSubscriptions
            .GetSubscriptions(publication)
            .OrderBy(x => x.ExecutionOrder);

         foreach (var subscription in matching) {
            subscription.Invoke(publication);
         }
      }
   }
}
