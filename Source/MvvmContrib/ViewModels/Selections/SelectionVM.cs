namespace Inspiring.Mvvm.ViewModels {

   public abstract class SelectionVM<TDescriptor, TItemSource, TItemVM> :
      ViewModel<TDescriptor>
      where TDescriptor : SelectionVMDescriptor<TItemSource, TItemVM>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      protected SelectionVM(
         TDescriptor descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      internal bool AllItemsLoaded {
         get { return Kernel.IsLoaded(Descriptor.AllItems); }
      }

      internal TItemVM GetItemVM(TItemSource source) {
         var cacheBehavior = Descriptor
            .Behaviors
            .GetNextBehavior<SelectionItemViewModelCacheBehavior<TItemSource, TItemVM>>();

         return cacheBehavior.GetVMForSource(GetContext(), source);
      }
   }
}
