namespace Inspiring.Mvvm.ViewModels {
   public class SelectionItemVM<TSourceItem> :
      ViewModel<VMDescriptor>,
      ICanInitializeFrom<TSourceItem> {

      public TSourceItem SourceItem { get; private set; }

      public void InitializeFrom(TSourceItem source) {
         SourceItem = source;
      }

      public override string ToString() {
         var desc = _descriptor as SelectionItemVMDescriptor;
         
         return desc != null ?
            GetValue(desc.Caption) :
            base.ToString();
      }
   }

   public class SelectionItemVMDescriptor : VMDescriptor {
      public VMProperty<string> Caption { get; set; }
   }
}
