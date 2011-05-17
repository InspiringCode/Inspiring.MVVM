namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;

   public class SingleSelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      internal IVMPropertyDescriptor<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      internal IVMPropertyDescriptor<TItemSource> SelectedSourceItem { get; set; }
      public IVMPropertyDescriptor<IVMCollection<TItemVM>> AllItems { get; set; }
      public IVMPropertyDescriptor<TItemVM> SelectedItem { get; set; }
   }

   public class SingleSelectionVMDescriptor<TItemSource> :
      SingleSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
