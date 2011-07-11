namespace Inspiring.Mvvm.Common {
   using System.Collections.Generic;

   public sealed class EventAggregator : IEventSubscriptionRepository {



      internal void Publish<TPayload>(Event<TPayload> p, TPayload payload) {
         throw new System.NotImplementedException();
      }

      //public void AddSubscriptionStore(IEnumerable<IEventSubscription> store) {
      //   throw new System.NotImplementedException();
      //}

      void IEventSubscriptionRepository.AddSubscriptionStore(IEnumerable<IEventSubscription> store) {
         throw new System.NotImplementedException();
      }

      //private class SubscriptionStoreReference {
      //   public IEnumerable<IEventSubscription> Store { g
      //}
   }
}
