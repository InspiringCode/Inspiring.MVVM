namespace Inspiring.Mvvm.Common {
   using System;
   using System.Linq;
   using System.Collections.Generic;

   public abstract class HierarchicalEvent<TTarget, TArgs> :
      IEvent<TArgs>
      where TArgs : HierarchicalEventArgs<TTarget> {

      void IEvent<TArgs>.Publish(
         IEventSubscriptionRepository allSubscriptions,
         TArgs payload
      ) {
         var publication = new EventPublication<TArgs>(this, payload);
         
         foreach (TTarget node in GetHierarchyNodes(payload.Target)) {
            PublishToSingleNode(allSubscriptions, node, publication);
         }
      }

      protected abstract IEnumerable<TTarget> GetHierarchyNodes(TTarget root);

      private void PublishToSingleNode(
         IEventSubscriptionRepository allSubscriptions,
         TTarget node, 
         EventPublication<TArgs> publication
      ) {
         IEnumerable<IEventSubscription<TArgs>> matching = allSubscriptions
            .GetSubscriptions(publication)
            .OfType<HierarchicalEventSubscription<TTarget, TArgs>>()
            .Where(s => Object.ReferenceEquals(s.Target, node))
            .OrderBy(x => x.ExecutionOrder);

         foreach (var subscription in matching) {
            subscription.Invoke(publication.Payload);
         }
      }
   }

   public class HierarchicalEventArgs<TTarget> : EventArgs {
      public HierarchicalEventArgs(TTarget target) {
         Target = target;
      }

      public TTarget Target { get; private set; }
   }
}
