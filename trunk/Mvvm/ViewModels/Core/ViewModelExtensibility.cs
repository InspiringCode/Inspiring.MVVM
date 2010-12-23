using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   public static class ViewModelExtensibility {
      public static BehaviorConfigurationDictionary ExposeBehaviorConfigurations(
         IVMPropertyFactory propertyFactory
      ) {
         // TODO: Clean up...
         return ((IBehaviorConfigurationDictionaryProvider)propertyFactory).Provide();
      }

      public static T GetForeignProerty<T>(
         IViewModel foreignViewModel,
         VMPropertyBase<T> property
      ) {
         throw new NotImplementedException();
         //return property.GetValue(foreignViewModel);
      }

      public static void SetForeignProperty<T>(
         IViewModel foreignViewModel,
         VMPropertyBase<T> property,
         T value
      ) {
         throw new NotImplementedException();
         //property.SetValue(foreignViewModel, value);
      }
   }
}
