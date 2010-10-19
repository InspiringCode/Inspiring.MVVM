namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class BehaviorConfigurationBuilder : IVMBehaviorConfigurator {
      private BehaviorConfigurationDictionary _configs;

      public BehaviorConfigurationBuilder(BehaviorConfigurationDictionary configs) {
         Contract.Requires(configs != null);
         _configs = configs;
      }

      public void Custom<T>(VMPropertyBase<T> property, VMBehaviorKey behaviorToEnable) {
         BehaviorConfiguration config = _configs.GetConfiguration(property);
         config.Enable(behaviorToEnable);
      }
   }
}
