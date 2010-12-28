namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;

   internal sealed class PropertyTypeHelper {
      public static bool IsViewModel(Type propertyType) {
         return typeof(IViewModel).IsAssignableFrom(propertyType);
      }

      public static bool IsViewModelCollection(Type propertyType) {
         if (IsVMCollectionInterface(propertyType)) {
            return true;
         }

         bool implementsInterface =
            propertyType
               .GetInterfaces()
               .Any(IsVMCollectionInterface);

         return implementsInterface;
      }

      private static bool IsVMCollectionInterface(Type type) {
         return
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(IVMCollection<>);
      }
   }
}
