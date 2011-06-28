namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IActionOrAnyDescendantBuilder<TRootVM, TRootDescriptor> :
      IDependencyActionBuilder<TRootVM, TRootDescriptor>
      where TRootVM : IViewModel
      where TRootDescriptor : IVMDescriptor {

      IDependencyActionBuilder<TRootVM, TRootDescriptor> OrAnyDescendant { get; }
   }
}
