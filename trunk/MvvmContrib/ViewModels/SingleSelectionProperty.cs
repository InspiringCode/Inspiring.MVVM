using Inspiring.Mvvm.ViewModels.Core;
namespace Inspiring.Mvvm.ViewModels {
   public class SingleSelectionProperty<TSourceItem> :
      VMProperty<SingleSelectionVM<TSourceItem, SelectionItemVM<TSourceItem>>>,
      IBindableProperty<ViewModel<SingleSelectionVMDescriptor<TSourceItem, SelectionItemVM<TSourceItem>>>> {
   }

   public class SingleSelectionProperty<TSourceItem, TItemVM> :
      VMProperty<SingleSelectionVM<TSourceItem, TItemVM>>
      where TItemVM : SelectionItemVM<TSourceItem> {
   }
}
