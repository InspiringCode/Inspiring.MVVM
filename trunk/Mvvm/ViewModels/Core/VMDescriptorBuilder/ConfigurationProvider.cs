namespace Inspiring.Mvvm.ViewModels.Core {
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public abstract class ConfigurationProvider : IConfigurationProvider {
      public ConfigurationProvider(VMDescriptorConfiguration configuration) {
         Contract.Requires(configuration != null);
         Configuration = configuration;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      public VMDescriptorConfiguration Configuration {
         get;
         private set;
      }
   }
}
