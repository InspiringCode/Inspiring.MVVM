namespace Inspiring.Mvvm.Common {

   public interface IEventSubscription {
      /// <summary>
      ///   The event for which this subscription listens. Null if the subscription
      ///   does not listen for a specific event.
      /// </summary>
      IEvent Event { get; }

      ExecutionOrder ExecutionOrder { get; }
      bool Matches(EventPublication publication);
      void Invoke(EventPublication publication);
   }
}
