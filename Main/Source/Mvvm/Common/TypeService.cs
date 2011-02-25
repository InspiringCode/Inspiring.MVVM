namespace Inspiring.Mvvm.Common {
   using System;

   internal static class TypeService {
      public static bool CanAssignNull(Type type) {
         return !type.IsValueType || IsNullableType(type);
      }

      public static bool IsNullableType(Type type) {
         return
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>);
      }
   }
}
