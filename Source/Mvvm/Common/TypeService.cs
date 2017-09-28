namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
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
         return ClosesGenericType(type, typeof(Nullable<>));
      }

      public static Type GetItemType(Type collectionType) {
         Check.NotNull(collectionType, nameof(collectionType));

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

      public static bool ClosesGenericType(Type t, Type generic) {
         Check.Requires(generic.IsGenericTypeDefinition);

         return
            t.IsGenericType &&
            t.GetGenericTypeDefinition() == generic;
      }

      private static bool IsEnumerableInterface(Type interfaceType) {
         return ClosesGenericType(interfaceType, typeof(IEnumerable<>));
      }
   }
}
