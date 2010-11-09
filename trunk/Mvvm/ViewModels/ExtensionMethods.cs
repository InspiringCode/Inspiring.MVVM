namespace Inspiring.Mvvm.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class ExtensionMethods {
      public static void CallNext<T>(this T behavior, Action<T> methodSelector) where T : IBehavior {

      }

      public static TResult CallNext<T, TResult>(this T behavior, Func<T, TResult> methodSelector) where T : IBehavior {
         return default(TResult);
      }
   }
}
