namespace Inspiring.Mvvm {

   internal static class ArrayUtils {
      public static T[] Concat<T>(T[] first, T[] second) {
         T[] result = new T[first.Length + second.Length];

         first.CopyTo(result, 0);
         second.CopyTo(result, first.Length);

         return result;
      }
   }
}
