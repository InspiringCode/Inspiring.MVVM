namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class BehaviorConfigurationBuilder : IVMBehaviorConfigurator {
      private VMDescriptorConfiguration _configuration;

      public BehaviorConfigurationBuilder(VMDescriptorConfiguration configuration) {
         Contract.Requires(configuration != null);
         _configuration = configuration;
      }

      public void Custom<T>(IVMProperty<T> property, VMBehaviorKey behaviorToEnable) {
         throw new NotImplementedException();
         //BehaviorConfiguration config = _configuration.GetConfiguration(property);
         //config.Enable(behaviorToEnable);
      }
   }
}
