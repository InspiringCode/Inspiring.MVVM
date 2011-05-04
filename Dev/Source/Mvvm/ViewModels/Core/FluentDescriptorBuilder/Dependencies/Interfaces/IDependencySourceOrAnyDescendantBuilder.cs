namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IDependencySourceOrAnyDescendantBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor> :
      IDependencySourceBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>,
      IActionOrAnyDescendantBuilder<TRootVM, TRootDescriptor>
      where TRootVM : IViewModel
      where TSourceVM : IViewModel
      where TRootDescriptor : VMDescriptorBase
      where TSourceDescriptor : VMDescriptorBase {

   }
}
