namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   public class MultiSelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      public IVMProperty<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      public IVMProperty<ICollection<TItemSource>> SelectedSourceItems { get; set; }
      public IVMProperty<IVMCollection<TItemVM>> AllItems { get; set; }
      public IVMProperty<IVMCollection<TItemVM>> SelectedItems { get; set; }
   }

   public class MultiSelectionVMDescriptor<TItemSource> :
      MultiSelectionVMDescriptor<TItemSource, SelectionItemVM<TItemSource>> {
   }
}
