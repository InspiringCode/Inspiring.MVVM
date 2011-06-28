namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;

   public class MultiSelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      internal IVMPropertyDescriptor<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      internal IVMPropertyDescriptor<ICollection<TItemSource>> SelectedSourceItems { get; set; }
      public IVMPropertyDescriptor<IVMCollection<TItemVM>> AllItems { get; set; }
      public IVMPropertyDescriptor<IVMCollection<TItemVM>> SelectedItems { get; set; }
   }

   public class MultiSelectionVMDescriptor<TItemSource> :
      MultiSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
