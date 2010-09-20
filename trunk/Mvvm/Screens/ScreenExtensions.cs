namespace Inspiring.Mvvm.Screens {
   using System;

   public static class ScreenExtensions {
      public static bool GetDialogWindowResult(this Screen dialog) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);

         if (!dl.WindowResult.HasValue) {
            throw new InvalidOperationException(ExceptionTexts.WindowDialogResultNotAssigned);
         }

         return dl.WindowResult.Value;
      }

      public static void SetDialogResult(this Screen dialog, DialogScreenResult result) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         dl.ScreenResult = result;
      }

      public static void CloseDialog(this Screen dialog, DialogScreenResult result) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         dl.ScreenResult = result;
         dl.RaiseCloseWindow();
      }


   }
}
