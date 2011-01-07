namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   public sealed class BehaviorConfigurationDictionary {
      private Dictionary<IVMProperty, BehaviorConfiguration> _configurations
         = new Dictionary<IVMProperty, BehaviorConfiguration>();

      public BehaviorConfiguration GetConfiguration(IVMProperty forProperty) {
         return _configurations[forProperty];
      }

      public void Add(IVMProperty property, BehaviorConfiguration configuration) {
         _configurations.Add(property, configuration);
      }

      internal void ApplyToProperties(VMDescriptor descriptor) {
         throw new NotImplementedException();
         //foreach (var pair in _configurations) {
         //   VMPropertyBase property = pair.Key;
         //   BehaviorConfiguration config = pair.Value;

         //   property.ConfigureBehaviors(config, descriptor);
         //}
      }
   }
}
