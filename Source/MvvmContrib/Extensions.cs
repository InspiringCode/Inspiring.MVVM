namespace Inspiring.Mvvm {
   using System;
   using System.Collections.Generic;
   
   internal static class Extensions {
      public static void ForEach<TValue>(this IEnumerable<TValue> collection, Action<TValue> func) {
         if (collection != null) {
            foreach (TValue item in collection) {
               func(item);
            }
         }
      }

      //public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, T value) {
      //   return collection.Concat(Enumerable.Repeat(value, 1));
      //}

      public static string FormatWith(this string format, params object[] args) {
         Check.NotNull(format, nameof(format));
         Check.NotNull(args, nameof(args));

         return string.Format(format, args);
      }
   }
}
