namespace Inspiring.Mvvm.Screens {
   using Inspiring.Mvvm.Common;

   public sealed class ScreenEvent<TArgs> :
      IEvent<TArgs>
      where TArgs : ScreenEventArgs_ {



      void IEvent<TArgs>.Publish(
         IEventSubscriptionRepository allSubscriptions, 
         TArgs payload
      ) {
         
         throw new System.NotImplementedException();
      }
   }
}
