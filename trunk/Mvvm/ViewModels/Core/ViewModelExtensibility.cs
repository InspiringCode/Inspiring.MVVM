namespace Inspiring.Mvvm.ViewModels.Core {

   public static class ViewModelExtensibility {
      public static BehaviorConfigurationDictionary ExposeBehaviorConfigurations(
         IVMPropertyFactory propertyFactory
      ) {
         // TODO: Clean up...
         return ((IBehaviorConfigurationDictionaryProvider)propertyFactory).Provide();
      }

      public static T GetForeignProerty<T>(
         ViewModel foreignViewModel,
         VMPropertyBase<T> property
      ) {
         return property.GetValue(foreignViewModel);
      }

      public static void SetForeignProperty<T>(
         ViewModel foreignViewModel,
         VMPropertyBase<T> property,
         T value
      ) {
         property.SetValue(foreignViewModel, value);
      }

      public static IRootVMPropertyFactory<TVM> ConfigurePropertyFactory<TVM>(
         IRootVMPropertyFactory<TVM> factory,
         BehaviorConfiguration additionalConfiguration
      ) where TVM : ViewModel {
         return ((VMPropertyFactory<TVM, TVM>)factory).WithConfiguration(additionalConfiguration);
      }

      //public static IRootVMPropertyFactory<TVM> CreatePropertyFactory<TVM>() where TVM : ViewModel {
      //   return new VMPropertyFactory<TVM, TVM>(PropertyPath.Empty<TVM>, 
      //}
   }
}
