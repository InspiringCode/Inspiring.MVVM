namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   public class MultiSelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      public VMProperty<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      public VMProperty<ICollection<TItemSource>> SelectedSourceItems { get; set; }
      public VMProperty<IVMCollection<TItemVM>> AllItems { get; set; }
      public VMProperty<IVMCollection<TItemVM>> SelectedItems { get; set; }
   }

   public class MultiSelectionVMDescriptor<TItemSource> :
      MultiSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
