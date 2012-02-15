namespace Inspiring.Mvvm.Common {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class HierarchicalEventSubscription<TTarget, TArgs> : EventSubscription<TArgs> {
      private readonly TTarget _target;

      public HierarchicalEventSubscription(
         IEvent<TArgs> @event,
         Action<TArgs> handler,
         ExecutionOrder executionOrder,
         IEventCondition<TArgs>[] conditions,
         TTarget target
      )
         : base(@event, handler, executionOrder, conditions) {

         Contract.Requires<ArgumentNullException>(target != null);
         _target = target;
      }

      public TTarget Target {
         get { return _target; }
      }
   }
}
