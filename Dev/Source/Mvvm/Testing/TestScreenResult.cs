namespace Inspiring.Mvvm.Testing {
   using Inspiring.Mvvm.Screens;

   public class TestScreenResult {
      private readonly DialogLifecycle dialogLifecycle;

      internal TestScreenResult(DialogLifecycle lifecycle) {
         dialogLifecycle = lifecycle;
         dialogLifecycle.CloseWindow += (s, e) => {
            Closed = true;
         };
      }

      public bool Closed { get; private set; }

      public DialogScreenResult ScreenResult {
         get { return dialogLifecycle.ScreenResult; }
      }
   }
}