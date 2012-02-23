namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;

   internal static class ScreenTreeHelper {
      public static IEnumerable<IScreenBase> GetAncestorsOf(IScreenBase screen) {
         for (IScreenBase s = screen; s != null; s = s.Parent) {
            yield return s;
         }
      }
   }
}
