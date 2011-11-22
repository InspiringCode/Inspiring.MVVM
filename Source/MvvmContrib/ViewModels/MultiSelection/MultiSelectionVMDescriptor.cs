namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;

   public class MultiSelectionVMDescriptor<TItemSource, TItemVM> :
      SelectionVMDescriptor<TItemSource, TItemVM>
      where TItemVM : IViewModel {

      internal IVMPropertyDescriptor<ICollection<TItemSource>> SelectedSourceItems { get; set; }
      public IVMPropertyDescriptor<IVMCollection<TItemVM>> SelectedItems { get; set; }
   }

   public class MultiSelectionVMDescriptor<TItemSource> :
      MultiSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
