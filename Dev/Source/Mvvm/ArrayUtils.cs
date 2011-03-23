namespace Inspiring.Mvvm {

   internal static class ArrayUtils {
      public static T[] Prepend<T>(T[] array, T item) {
         var result = new T[array.Length + 1];

         result[0] = item;
         array.CopyTo(result, 1);

         return result;
      }

      public static T[] Append<T>(T[] array, T item) {
         var result = new T[array.Length + 1];

         array.CopyTo(result, index: 0);
         result[result.Length - 1] = item;

         return result;
      }

      public static T[] Concat<T>(T[] first, T[] second) {
         T[] result = new T[first.Length + second.Length];

         first.CopyTo(result, 0);
         second.CopyTo(result, first.Length);

         return result;
      }
   }
}
