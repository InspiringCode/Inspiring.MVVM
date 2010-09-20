namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class DialogLifecycle : ScreenLifecycle {
      public Nullable<bool> WindowResult { get; set; }

      public DialogScreenResult ScreenResult { get; set; }

      public event EventHandler CloseWindow;

      public static DialogLifecycle GetDialogLifecycle(Screen forScreen) {
         if (!forScreen.Children.Contains<DialogLifecycle>()) {
            throw new ArgumentException(ExceptionTexts.ScreenIsNoDialog);
         }

         return forScreen.Children.Expose<DialogLifecycle>();
      }

      public void RaiseCloseWindow() {
         Contract.Assert(CloseWindow != null);
         CloseWindow(this, EventArgs.Empty);
      }
   }
}
