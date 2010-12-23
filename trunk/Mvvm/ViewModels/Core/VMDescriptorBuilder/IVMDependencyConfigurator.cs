namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.Common;

   public interface IVMDependencyConfigurator<TDescriptor> :
      IHideObjectMembers
      where TDescriptor : VMDescriptor {

   }
}
