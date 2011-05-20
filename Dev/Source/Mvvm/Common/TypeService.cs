namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
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

      public static string GetFriendlyTypeName(object instance) {
         return instance == null ?
            "<NULL>" :
            GetFriendlyName(instance.GetType());
      }

      public static bool CanAssignNull(Type type) {
         return !type.IsValueType || IsNullableType(type);
      }

      public static bool IsNullableType(Type type) {
         return
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>);
      }

      public static Type GetItemType(Type collectionType) {
         Contract.Requires(collectionType != null);

         Type enumerableInterface = collectionType
            .GetInterfaces()
            .FirstOrDefault(IsEnumerableInterface);

         if (enumerableInterface != null) {
            return enumerableInterface
               .GetGenericArguments()
               .Single();
         }

         return null;
      }

      private static bool IsEnumerableInterface(Type interfaceType) {
         return
            interfaceType.IsGenericType &&
            interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
      }
   }
}
