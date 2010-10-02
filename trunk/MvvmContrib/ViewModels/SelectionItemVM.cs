using System;
using System.Collections;
using System.Collections.Generic;
namespace Inspiring.Mvvm.ViewModels {
   public class SelectionItemVM<TSourceItem> :
      ViewModel<VMDescriptor>,
      ICanInitializeFrom<TSourceItem>,
      IComparable<SelectionItemVM<TSourceItem>> {

      public TSourceItem SourceItem { get; private set; }

      public void InitializeFrom(TSourceItem source) {
         SourceItem = source;
      }

      public override string ToString() {
         return GetCaption() ?? base.ToString();
      }

      public int CompareTo(SelectionItemVM<TSourceItem> other) {
         string caption = GetCaption();
         string otherCaption = other.GetCaption();

         return String.Compare(caption, otherCaption);
      }

      private string GetCaption() {
         var desc = _descriptor as SelectionItemVMDescriptor;

         return desc != null ?
            GetValue(desc.Caption) :
            null;
      }
   }

   public class SelectionItemVMDescriptor : VMDescriptor {
      public VMProperty<string> Caption { get; set; }
   }
}
