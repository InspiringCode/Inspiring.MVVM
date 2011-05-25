﻿namespace Inspiring.Mvvm.ViewModels {
   using System;

   public class SelectionItemVM<TSourceItem> :
      ViewModel<SelectionItemVMDescriptor>,
      IHasSourceObject<TSourceItem>,
      IComparable<SelectionItemVM<TSourceItem>> {

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
         return String.Compare(Caption, other.Caption);
      }
   }

   public class SelectionItemVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Caption { get; set; }
   }
}
