namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core.Kernel;

   public interface IViewModel {
      VMKernel Kernel { get; }
      object GetValue(IVMProperty property);
   }
}
