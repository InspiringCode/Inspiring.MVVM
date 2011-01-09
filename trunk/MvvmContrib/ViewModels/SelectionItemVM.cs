namespace Inspiring.Mvvm.ViewModels {
   using System;

   public class SelectionItemVM<TSourceItem> :
      ViewModel<SelectionItemVMDescriptor>,
      IVMCollectionItem<TSourceItem>,
      IComparable<SelectionItemVM<TSourceItem>> {

      // The descriptor is set by the collection
      public SelectionItemVM() {
      }

      public TSourceItem Source { get; private set; }

      public string Caption {
         get { return GetValue(Descriptor.Caption); }
      }

      public void InitializeFrom(TSourceItem source) {
         Source = source;
      }

      public override string ToString() {
         return Caption ?? base.ToString();
      }

      public int CompareTo(SelectionItemVM<TSourceItem> other) {
         return String.Compare(Caption, other.Caption);
      }
   }

   public class SelectionItemVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Caption { get; set; }
   }
}
