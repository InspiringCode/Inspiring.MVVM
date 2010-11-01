namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   public sealed class BehaviorConfigurationDictionary {
      private Dictionary<VMPropertyBase, BehaviorConfiguration> _configurations
         = new Dictionary<VMPropertyBase, BehaviorConfiguration>();

      public BehaviorConfiguration GetConfiguration(VMPropertyBase forProperty) {
         return _configurations[forProperty];
      }

      public void Add(VMPropertyBase property, BehaviorConfiguration configuration) {
         _configurations.Add(property, configuration);
      }

      internal void ApplyToProperties() {
         foreach (var pair in _configurations) {
            VMPropertyBase property = pair.Key;
            BehaviorConfiguration config = pair.Value;
            property.ConfigureBehaviors(config);
         }
      }
   }
}
