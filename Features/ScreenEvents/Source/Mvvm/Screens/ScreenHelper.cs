namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Linq;

   internal static class ScreenHelper {
      public static void Close(IScreenBase screen, bool skipRequestClose) {
         ScreenCloseHandler h = screen
            .Children
            .OfType<ScreenCloseHandler>()
            .SingleOrDefault();

         if (h == null) {
            throw new ArgumentException();
         }

         h.Execute(skipRequestClose);
      }
   }
}
