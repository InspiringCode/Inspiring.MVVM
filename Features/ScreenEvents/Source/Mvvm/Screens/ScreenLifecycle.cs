namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Diagnostics;

   public class ScreenLifecycle_ {
      // State transitions
      // etc.


      public void RegisterHandler<TArgs>(
         ScreenEvent<TArgs> @event,
         Action<TArgs> handler
      ) where TArgs : ScreenEventArgs_ {

         // State Created
         //  On Initialize transition to Initialized
      }

      private void test() {
         RegisterHandler(ScreenEvents.Initialize<Stopwatch>(), Initialize);
      }

      private void Initialize(InitializeEventArgs<Stopwatch> subject) {

      }
   }


   public abstract class ScreenLifecycle : IScreenLifecycle {
      public virtual IScreenLifecycle Parent { get; set; }

      public virtual void Activate() {
      }

      public virtual void Deactivate() {
      }

      public virtual bool RequestClose() {
         return true;
      }

      public virtual void Close() {
      }

      public void Corrupt(object data = null) {
      }
   }
}
