namespace Inspiring.Mvvm.ViewModels.Core {
   using System.ComponentModel;

   public abstract class ConfigurationProvider : IConfigurationProvider {
      public ConfigurationProvider(VMDescriptorConfiguration configuration) {
         Check.NotNull(configuration, nameof(configuration));
         Configuration = configuration;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      public VMDescriptorConfiguration Configuration {
         get;
         private set;
      }
   }
}
