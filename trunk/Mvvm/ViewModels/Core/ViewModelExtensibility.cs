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

      public static IRootVMPropertyFactory<TVM> ConfigurePropertyFactory<TVM>(
         IRootVMPropertyFactory<TVM> factory,
         BehaviorConfiguration additionalConfiguration
      ) where TVM : IViewModel {
         return ((_VMPropertyFactory<TVM, TVM>)factory).WithConfiguration(additionalConfiguration);
      }

      //public static IRootVMPropertyFactory<TVM> CreatePropertyFactory<TVM>() where TVM : IViewModel {
      //   return new VMPropertyFactory<TVM, TVM>(PropertyPath.Empty<TVM>, 
      //}
   }
}
