namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   public class MultiSelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      public IVMPropertyDescriptor<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      internal IVMPropertyDescriptor<ICollection<TItemSource>> SelectedSourceItems { get; set; }
      public IVMPropertyDescriptor<IVMCollection<TItemVM>> AllItems { get; set; }
      internal IVMPropertyDescriptor<IVMCollection<TItemVM>> SelectedItems { get; set; }
   }

   public class MultiSelectionVMDescriptor<TItemSource> :
      MultiSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
