namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class DialogLifecycle : ScreenLifecycle {
      public Nullable<bool> WindowResult { get; set; }

      public DialogScreenResult ScreenResult { get; set; }

      public event EventHandler CloseWindow;

      public static DialogLifecycle GetDialogLifecycle(IScreenBase forScreen) {
         var lifecycle = TryGetDialogLifecycle(forScreen);

         if (lifecycle == null) {
            throw new ArgumentException(ExceptionTexts.ScreenIsNoDialog);
         }

         return lifecycle;
      }

      public static bool HasDialogLifecycle(IScreenBase forScreen) {
         return TryGetDialogLifecycle(forScreen) != null;
      }

      private static DialogLifecycle TryGetDialogLifecycle(IScreenBase forScreen) {
         // TODO: Is this the best semantic?
         for (IScreenLifecycle s = forScreen; s != null; s = s.Parent) {
            IScreenBase scr = s as IScreenBase;
            if (scr != null && scr.Children.Contains<DialogLifecycle>()) {
               return scr.Children.Expose<DialogLifecycle>();
            }
         }

         return null;
      }

      public void RaiseCloseWindow() {
         Contract.Assert(CloseWindow != null);
         CloseWindow(this, EventArgs.Empty);
      }
   }
}
