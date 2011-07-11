namespace Inspiring.Mvvm.Common {

   public interface IEventSubscription {
      IEvent Event { get; }
   }

   public interface IEventSubscription<TPayload> : IEventSubscription {
      bool Matches(EventPublication<TPayload> publication);
      void Invoke(TPayload payload);
   }
}
