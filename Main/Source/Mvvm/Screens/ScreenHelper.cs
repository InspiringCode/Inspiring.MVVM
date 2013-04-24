namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Linq;

   public static class ScreenHelper {
      public static void Close(IScreenBase screen, bool requestClose = false) {
         ScreenCloseHandler h = GetChild<ScreenCloseHandler>(screen);
         h.Execute(requestClose);
      }

      public static void Close(IScreenBase dialog, DialogScreenResult result, bool requestClose = false) {
         SetDialogResult(dialog, result);
         Close(dialog, requestClose);
      }

      public static bool IsDialog(IScreenBase screen) {
         return DialogLifecycle.HasDialogLifecycle(screen);
      }

      public static DialogScreenResult GetDialogResult(IScreenBase dialog) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         return dl.ScreenResult;
      }

      public static void SetDialogResult(IScreenBase dialog, DialogScreenResult result) {
         DialogLifecycle dl = DialogLifecycle.GetDialogLifecycle(dialog);
         dl.ScreenResult = result;
      }

      internal static TChild GetChild<TChild>(IScreenBase screen) {
         TChild child = screen
            .Children
            .OfType<TChild>()
            .SingleOrDefault();

         if (child == null) {
            throw new ArgumentException();
         }

         return child;
      }
   }
}
