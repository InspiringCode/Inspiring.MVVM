namespace Inspiring.Mvvm.ViewModels.SingleSelection {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   public class SingleSelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      public IVMProperty<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      public IVMProperty<TItemSource> SelectedSourceItem { get; set; }
      public IVMProperty<IVMCollection<TItemVM>> AllItems { get; set; }
      public IVMProperty<TItemVM> SelectedItem { get; set; }
   }

   public class SingleSelectionVMDescriptor<TItemSource> :
      SingleSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
