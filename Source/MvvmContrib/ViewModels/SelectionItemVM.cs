namespace Inspiring.Mvvm.ViewModels {
   using System;

   public class SelectionItemVM<TSourceItem> :
      ViewModel<SelectionItemVMDescriptor>,
      IHasSourceObject<TSourceItem>,
      IComparable<SelectionItemVM<TSourceItem>>,
      IComparable {

      // The descriptor is set by the collection
      public SelectionItemVM() {
      }

      public TSourceItem Source { get; set; }

      public string Caption {
         get { return GetValue(Descriptor.Caption); }
      }

      public void InitializeFrom(TSourceItem source) {
         Source = source;
      }

      public override string ToString() {
         return Caption ?? String.Empty;
      }

      public int CompareTo(SelectionItemVM<TSourceItem> other) {
         return other != null ?
            String.Compare(Caption, other.Caption) :
            String.Compare(Caption, null);
      }

      public int CompareTo(object obj) {
         return CompareTo(obj as SelectionItemVM<TSourceItem>);
      }
   }

   public class SelectionItemVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Caption { get; set; }
   }
}
