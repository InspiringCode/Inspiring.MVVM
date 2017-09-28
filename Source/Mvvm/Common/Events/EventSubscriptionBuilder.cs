namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public class EventSubscriptionBuilder<TPayload> {
      private readonly SubscriptionBuilderInterface _builderInterface;

      public EventSubscriptionBuilder(SubscriptionBuilderInterface builderInterface) {
         Check.NotNull(builderInterface, nameof(builderInterface));

         _builderInterface = builderInterface;
         Conditions = new List<IEventCondition>();
      }

      public IEvent<TPayload> Event { get; set; }

      public ExecutionOrder ExecutionOrder { get; set; }

      public Action<TPayload> Handler { get; set; }

      public ICollection<IEventCondition> Conditions { get; private set; }

      public void Build() {
         IEventCondition[] conditions = Conditions.Any() ?
            Conditions.ToArray() :
            null;

         IEventSubscription s = CreateSubscription(conditions);

         _builderInterface.AddSubscription(s);
      }

      protected virtual IEventSubscription CreateSubscription(
         IEventCondition[] conditions
      ) {
         return new EventSubscription<TPayload>(
            Event,
            Handler,
            ExecutionOrder,
            conditions
         );
      }
   }
}
