namespace Inspiring.Mvvm.ViewModels.SingleSelection {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   public class SingleSelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      public VMProperty<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      public VMProperty<TItemSource> SelectedSourceItem { get; set; }
      public VMProperty<IVMCollection<TItemVM>> AllItems { get; set; }
      public VMProperty<TItemVM> SelectedItem { get; set; }
   }

   public class SingleSelectionVMDescriptor<TItemSource> :
      SingleSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
