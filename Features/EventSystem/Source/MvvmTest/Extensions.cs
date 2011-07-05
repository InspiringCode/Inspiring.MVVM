namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

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
         Contract.Requires(format != null);
         Contract.Requires(args != null);

         return string.Format(format, args);
      }
   }
}
