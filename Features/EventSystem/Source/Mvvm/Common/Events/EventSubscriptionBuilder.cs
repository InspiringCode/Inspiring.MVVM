namespace Inspiring.Mvvm.Common.Events {
   using System;
   using System.Diagnostics.Contracts;

   public class EventSubscriptionBuilder<TPayload> {
      private readonly SubscriptionBuilderInterface _builderInterface;

      public EventSubscriptionBuilder(SubscriptionBuilderInterface builderInterface) {
         Contract.Requires<ArgumentNullException>(builderInterface != null);
         _builderInterface = builderInterface;
      }

      public IEvent<TPayload> Event { get; set; }

      public ExecutionOrder ExecutionOrder { get; set; }

      public Action<TPayload> Handler { get; set; }

      //public ICollection<Func<TRegistration, TPayload, bool>> Filters { get; private set; }

      public void Create() {
         IEventSubscription s = CreateSubscription();
         _builderInterface.AddSubscription(s);
      }

      protected virtual IEventSubscription CreateSubscription() {
         return new EventSubscription<TPayload>(Event, Handler, ExecutionOrder);
      }
   }
}
