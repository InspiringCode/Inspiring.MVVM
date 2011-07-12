﻿namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   public class EventSubscriptionBuilder<TPayload> {
      private readonly SubscriptionBuilderInterface _builderInterface;

      public EventSubscriptionBuilder(SubscriptionBuilderInterface builderInterface) {
         Contract.Requires<ArgumentNullException>(builderInterface != null);

         _builderInterface = builderInterface;
         Conditions = new List<IEventCondition<TPayload>>();
      }

      public IEvent<TPayload> Event { get; set; }

      public ExecutionOrder ExecutionOrder { get; set; }

      public Action<TPayload> Handler { get; set; }

      public ICollection<IEventCondition<TPayload>> Conditions { get; private set; }

      public void Build() {
         IEventSubscription s = CreateSubscription();
         _builderInterface.AddSubscription(s);
      }

      protected virtual IEventSubscription CreateSubscription() {
         IEventCondition<TPayload>[] conditions = Conditions.Any() ?
            Conditions.ToArray() :
            null;

         return new EventSubscription<TPayload>(
            Event,
            Handler,
            ExecutionOrder,
            conditions
         );
      }
   }
}
