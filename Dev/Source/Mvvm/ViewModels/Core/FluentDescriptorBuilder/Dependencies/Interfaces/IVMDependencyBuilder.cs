namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.Common;

   public interface IVMDependencyBuilder<TRootVM, TRootDescriptor> :
      IHideObjectMembers
      where TRootVM : IViewModel
      where TRootDescriptor : VMDescriptor {

      IDependencySourceBuilder<TRootVM, TRootVM, TRootDescriptor, TRootDescriptor> OnChangeOf { get; }
   }
}
