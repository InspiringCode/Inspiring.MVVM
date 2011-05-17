namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   public class SingleSelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      public IVMPropertyDescriptor<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      internal IVMPropertyDescriptor<TItemSource> SelectedSourceItem { get; set; }
      public IVMPropertyDescriptor<IVMCollection<TItemVM>> AllItems { get; set; }
      internal IVMPropertyDescriptor<TItemVM> SelectedItem { get; set; }
   }

   public class SingleSelectionVMDescriptor<TItemSource> :
      SingleSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
