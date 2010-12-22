using System;
namespace Inspiring.Mvvm.ViewModels {
   public class SelectionItemVM<TSourceItem> :
      ViewModel<VMDescriptor>,
      IVMCollectionItem<TSourceItem>,
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
         throw new NotImplementedException();
         //var desc = _descriptor as SelectionItemVMDescriptor;

         //return desc != null ?
         //   GetValue(desc.Caption) :
         //   null;
      }

      TSourceItem IVMCollectionItem<TSourceItem>.Source {
         get { return SourceItem; }
      }
   }

   public class SelectionItemVMDescriptor : VMDescriptor {
      public VMProperty<string> Caption { get; set; }
   }
}
