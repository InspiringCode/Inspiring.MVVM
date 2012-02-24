namespace Inspiring.Mvvm.Screens {
   using System;
   using Inspiring.Mvvm.Common;

   internal class ScreenLifecycleOperations {
      private EventAggregator _aggregator;
      private IScreenBase _target;

      public ScreenLifecycleOperations(
         EventAggregator aggregator,
         IScreenBase target
      ) {
         _aggregator = aggregator;
         _target = target;
      }

      public void Initialize() {
         PublishEvent(ScreenEvents.Initialize(), new InitializeEventArgs(_target));
      }

      public void Initialize<TSubject>(TSubject subject) {
         PublishEvent(
            ScreenEvents.Initialize<TSubject>(),
            new InitializeEventArgs<TSubject>(_target, subject)
         );
      }

      public void Activate() {
         PublishEvent(ScreenEvents.Activate, new ScreenEventArgs(_target));
      }

      public void Deactivate() {
         PublishEvent(ScreenEvents.Deactivate, new ScreenEventArgs(_target));
      }

      public bool RequestClose() {
         RequestCloseEventArgs args = new RequestCloseEventArgs(_target);
         PublishEvent(ScreenEvents.RequestClose, args);
         return args.IsCloseAllowed;
      }

      public void Close() {
         PublishEvent(ScreenEvents.Close, new ScreenEventArgs(_target));
      }

      private void PublishEvent<TArgs>(
         ScreenEvent<TArgs> @event,
         TArgs args
      ) where TArgs : ScreenEventArgs {
         try {
            _aggregator.Publish(@event, args);
         } catch (Exception ex) {
            if (ex.IsCritical()) {
               throw;
            }

            _aggregator.Publish(
               ScreenEvents.LifecycleExceptionOccured,
               new ScreenEventArgs(_target)
            );
         }
      }
   }
}
