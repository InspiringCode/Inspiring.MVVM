namespace Inspiring.Mvvm.Common {
   using System;
   using System.Linq;

   internal static class TypeService {
      public static string GetFriendlyName(Type type) {
         string name = type.Name;

         if (type.IsGenericType) {
            var genericArgumentNames = type
               .GetGenericArguments()
               .Select(GetFriendlyName);

            name = name.Remove(startIndex: name.IndexOf("`"));

            name = String.Format(
               "{0}<{1}>",
               name,
               String.Join(", ", genericArgumentNames)
            );
         }

         return name;
      }

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
