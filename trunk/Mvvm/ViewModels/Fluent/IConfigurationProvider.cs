namespace Inspiring.Mvvm.ViewModels.Fluent {
   using System.ComponentModel;
   using Inspiring.Mvvm.ViewModels.Core;

   // TODO: Comment me
   public interface IConfigurationProvider {
      // TODO: Comment me
      [EditorBrowsable(EditorBrowsableState.Never)]
      VMDescriptorConfiguration Configuration { get; }
   }
}
