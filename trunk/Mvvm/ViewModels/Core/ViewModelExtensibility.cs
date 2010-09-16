namespace Inspiring.Mvvm.ViewModels.Core {

   public static class ViewModelExtensibility {
      public static BehaviorConfigurationDictionary ExposeBehaviorConfigurations(
         IVMPropertyFactory propertyFactory
      ) {
         // TODO: Clean up...
         return ((IBehaviorConfigurationDictionaryProvider)propertyFactory).Provide();
      }
   }
}
