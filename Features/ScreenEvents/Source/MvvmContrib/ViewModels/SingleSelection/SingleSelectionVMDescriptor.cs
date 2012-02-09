namespace Inspiring.Mvvm.ViewModels {
   public class SingleSelectionVMDescriptor<TItemSource, TItemVM> :
      SelectionVMDescriptor<TItemSource, TItemVM>
      where TItemVM : IViewModel {

      internal IVMPropertyDescriptor<TItemSource> SelectedSourceItem { get; set; }
      public IVMPropertyDescriptor<TItemVM> SelectedItem { get; set; }
   }

   public class SingleSelectionVMDescriptor<TItemSource> :
      SingleSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
