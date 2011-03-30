namespace Inspiring.Mvvm.Screens {
   using System;

   public static class ScreenExtensions {
      public static bool GetDialogWindowResult(this IScreenBase dialog) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);

         if (!dl.WindowResult.HasValue) {
            throw new InvalidOperationException(ExceptionTexts.WindowDialogResultNotAssigned);
         }

         return dl.WindowResult.Value;
      }

      public static DialogScreenResult GetDialogResult(this IScreenBase dialog) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         return dl.ScreenResult;
      }

      public static void SetDialogResult(this IScreenBase dialog, DialogScreenResult result) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         dl.ScreenResult = result;
      }

      public static void SetDialogResult(this IScreenBase dialog, Exception exception) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         dl.ScreenResult = new DialogScreenResult(false, new ExceptionResult(exception));
      }

      public static void CloseDialog(this IScreenBase dialog, DialogScreenResult result) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         dl.ScreenResult = result;
         dl.RaiseCloseWindow();
      }

      public static void CloseDialog(this IScreenBase dialog, Exception exception) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         dl.ScreenResult = new DialogScreenResult(false, new ExceptionResult(exception));
         dl.RaiseCloseWindow();
      }

      public static bool IsDialog(this IScreenBase screen) {
         return DialogLifecycle.HasDialogLifecycle(screen);
      }
   }
}
