namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class DialogLifecycle : ScreenLifecycle {
      public Nullable<bool> WindowResult { get; set; }

      public DialogScreenResult ScreenResult { get; set; }

      public event EventHandler CloseWindow;

      public static DialogLifecycle GetDialogLifecycle(IScreenBase forScreen) {
         // TODO: Is this the best semantic?
         for (IScreenLifecycle s = forScreen; s != null; s = s.Parent) {
            IScreenBase scr = s as IScreenBase;
            if (scr != null && scr.Children.Contains<DialogLifecycle>()) {
               return scr.Children.Expose<DialogLifecycle>();
            }
         }

         throw new ArgumentException(ExceptionTexts.ScreenIsNoDialog);
      }

      public void RaiseCloseWindow() {
         Contract.Assert(CloseWindow != null);
         CloseWindow(this, EventArgs.Empty);
      }
   }
}
