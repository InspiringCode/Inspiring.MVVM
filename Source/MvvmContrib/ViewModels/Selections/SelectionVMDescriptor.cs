namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;

   public abstract class SelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      internal IVMPropertyDescriptor<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      public IVMPropertyDescriptor<IVMCollection<TItemVM>> AllItems { get; set; }
   }
}
