namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   /// <remarks>
   ///   Derive from this class, if you want to write your own extension methods for
   ///   registering events or if you do not want to provide convenient registration
   ///   methods at all. Derive from <see cref="HierarchicalEvent{TTarget, TArgs}"/>
   ///   otherwise.
   /// </remarks>
   public abstract class HierarchicalEventBase<TTarget, TArgs> :
      IEvent<TArgs>
      where TArgs : HierarchicalEventArgs<TTarget> {

      void IEvent<TArgs>.Publish(
         IEventSubscriptionRepository allSubscriptions,
         TArgs payload
      ) {
         var publication = new EventPublication(this, payload);

         foreach (TTarget node in GetHierarchyNodes(payload.Target)) {
            PublishToSingleNode(allSubscriptions, node, publication);
         }
      }

      protected abstract IEnumerable<TTarget> GetHierarchyNodes(TTarget root);

      private void PublishToSingleNode(
         IEventSubscriptionRepository allSubscriptions,
         TTarget node,
         EventPublication publication
      ) {
         IEnumerable<IEventSubscription> matching = allSubscriptions
            .GetSubscriptions(publication)
            .OfType<IHierarchicalEventSubscription<TTarget>>()
            .Where(s => Object.ReferenceEquals(s.Target, node))
            .OrderBy(x => x.ExecutionOrder);

         foreach (var subscription in matching) {
            subscription.Invoke(publication);
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
