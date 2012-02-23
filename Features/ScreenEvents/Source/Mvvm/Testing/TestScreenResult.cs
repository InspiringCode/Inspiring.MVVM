namespace Inspiring.Mvvm.Testing {
   using Inspiring.Mvvm.Screens;

   public class TestScreenResult {
      private readonly DialogLifecycle dialogLifecycle;

      internal TestScreenResult(DialogLifecycle lifecycle) {
         dialogLifecycle = lifecycle;
      }

      public DialogScreenResult ScreenResult {
         get { return dialogLifecycle.ScreenResult; }
      }
   }
}