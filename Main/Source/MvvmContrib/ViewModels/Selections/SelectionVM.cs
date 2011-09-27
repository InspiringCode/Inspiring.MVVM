namespace Inspiring.Mvvm.ViewModels {

   public abstract class SelectionVM<TDescriptor, TItemSource, TItemVM> :
      ViewModel<TDescriptor>
      where TDescriptor : IVMDescriptor
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      protected SelectionVM(
         TDescriptor descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      internal TItemVM GetItemVM(TItemSource source) {
         var cacheBehavior = Descriptor
            .Behaviors
            .GetNextBehavior<SelectionItemViewModelCacheBehavior<TItemSource, TItemVM>>();

         return cacheBehavior.GetVMForSource(GetContext(), source);
      }
   }
}
