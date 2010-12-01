namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class VMDescriptorConfiguration {
      public VMDescriptorConfiguration(BehaviorChainTemplateKey viewModelBehaviorTemplate) {

      }

      public BehaviorChainConfigurationCollection PropertyConfigurations { get; private set; }

      public BehaviorChainConfiguration ViewModelConfiguration { get; private set; }
   }
}
