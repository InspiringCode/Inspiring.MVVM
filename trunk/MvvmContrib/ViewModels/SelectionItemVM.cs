namespace Inspiring.Mvvm.ViewModels {
   public sealed class SelectionItemVM<TSourceItem> :
      ViewModel<VMDescriptor>,
      ICanInitializeFrom<TSourceItem> {

      public SelectionItemVM(VMDescriptor descriptor)
         : base(descriptor) {
      }

      public TSourceItem SourceItem { get; private set; }

      public void InitializeFrom(TSourceItem source) {
         SourceItem = source;
      }
   }
}
