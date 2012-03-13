namespace Inspiring.Mvvm {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Threading;

   internal static class Extensions {
      public static void ForEach<TValue>(this IEnumerable<TValue> collection, Action<TValue> func) {
         if (collection != null) {
            foreach (TValue item in collection) {
               func(item);
            }
         }
      }

      public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, T value) {
         return collection.Concat(Enumerable.Repeat(value, 1));
      }

      public static string FormatWith(this string format, params object[] args) {
         Contract.Requires(format != null);
         Contract.Requires(args != null);

         return string.Format(format, args);
      }

      public static bool IsCritical(this Exception ex) {
         return
            ex is StackOverflowException ||
            ex is OutOfMemoryException ||
            ex is ThreadAbortException ||
            ex is AccessViolationException ||
            ex is InvalidProgramException;
      }

      public static IEnumerable<T> Traverse<T>(
         this T root,
         Func<T, T> getNext
      ) where T : class {
         yield return root;

         while ((root = getNext(root)) != null) {
            yield return root;
         }
      }

      public static IEnumerable<T> TraverseBreadthFirst<T>(
         this T root,
         Func<T, IEnumerable<T>> childrenSelector,
         bool includeRoot = false
      ) {
         List<T> children = new List<T>();

         if (includeRoot) {
            children.Add(root);
         }

         AddUnique(children, childrenSelector(root));

         int i = 0;

         while (i < children.Count) {
            yield return children[i];
            AddUnique(children, childrenSelector(children[i]));
            i++;
         }
      }

      private static void AddUnique<T>(List<T> list, IEnumerable<T> additional) {
         IEnumerable<T> unique = additional
            .Where(x => !list.Contains(x));

         list.AddRange(unique);
      }
   }
}
