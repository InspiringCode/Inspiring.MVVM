namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;

   internal sealed class PropertyTypeHelper {
      public static bool IsViewModel(Type propertyType) {
         return typeof(IViewModel).IsAssignableFrom(propertyType);
      }

      public static bool IsViewModelCollection(Type propertyType) {
         return propertyType
            .GetInterfaces()
            .Any(x =>
               x.IsGenericType &&
               x.GetGenericTypeDefinition() == typeof(IVMCollection<>)
            );
      }
   }
}
