namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IDependencySelfSourceBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor> :
      IDependencySourceBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>
      where TRootVM : IViewModel
      where TSourceVM : IViewModel
      where TRootDescriptor : IVMDescriptor
      where TSourceDescriptor : IVMDescriptor {

      IActionOrAnyDescendantBuilder<TRootVM, TRootDescriptor> Self { get; }
   }
}
