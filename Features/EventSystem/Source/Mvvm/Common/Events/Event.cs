namespace Inspiring.Mvvm.Common {
   using System.Collections.Generic;

   public class Event<TPayload> : IEvent<TPayload> {
      void IEvent<TPayload>.Publish(IEventSubscriptionRepository allSubscriptions, TPayload payload) {
         var publication = new EventPublication<TPayload>(this, payload);

         IEnumerable<IEventSubscription<TPayload>> matching = allSubscriptions
            .GetSubscriptions(publication);

         foreach (var subscription in matching) {
            subscription.Invoke(payload);
         }
      }
   }
}
