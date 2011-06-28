namespace Inspiring.Mvvm.ViewModels.Core {
   using System.ComponentModel;

   // TODO: Comment me
   public interface IConfigurationProvider {
      // TODO: Comment me
      [EditorBrowsable(EditorBrowsableState.Never)]
      VMDescriptorConfiguration Configuration { get; }
   }
}
