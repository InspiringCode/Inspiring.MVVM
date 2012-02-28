namespace Inspiring.Mvvm.Screens {

   public static class ScreenExtensions {
      public static DialogScreenResult GetDialogResult(this IScreenBase dialog) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         return dl.ScreenResult;
      }

      public static void SetDialogResult(this IScreenBase dialog, DialogScreenResult result) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         dl.ScreenResult = result;
      }

      public static void CloseDialog(this IScreenBase dialog, DialogScreenResult result) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         dl.ScreenResult = result;
         dl.RaiseCloseWindow();
      }

      public static bool IsDialog(this IScreenBase screen) {
         return DialogLifecycle.HasDialogLifecycle(screen);
      }
   }
}
