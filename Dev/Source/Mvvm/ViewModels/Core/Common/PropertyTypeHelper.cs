namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;

   // TODO: Still needed?
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

      public static bool IsCollectionPropertyDescriptor(Type propertyType) {
         return propertyType
            .GetGenericArguments()
            .Any(IsViewModelCollection);
      }

      private static bool IsVMCollectionInterface(Type type) {
         return
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(IVMCollection<>);
      }
   }
}
