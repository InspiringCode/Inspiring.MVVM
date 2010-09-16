namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   public sealed class BehaviorConfigurationDictionary {
      private Dictionary<VMProperty, BehaviorConfiguration> _configurations
         = new Dictionary<VMProperty, BehaviorConfiguration>();

      public BehaviorConfiguration GetConfiguration(VMProperty forProperty) {
         return _configurations[forProperty];
      }

      public void Add(VMProperty property, BehaviorConfiguration configuration) {
         _configurations.Add(property, configuration);
      }

      internal void ApplyToProperties() {
         foreach (var pair in _configurations) {
            VMProperty property = pair.Key;
            BehaviorConfiguration config = pair.Value;
            property.ConfigureBehaviors(config);
         }
      }
   }
}
