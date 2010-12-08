namespace Inspiring.Mvvm.ViewModels.Core.Builder {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal abstract class ConfigurationProvider : IConfigurationProvider {
      public ConfigurationProvider(VMDescriptorConfiguration configuration) {
         Contract.Requires(configuration != null);
         Configuration = configuration;
      }

      public VMDescriptorConfiguration Configuration {
         get;
         private set;
      }
   }
}
