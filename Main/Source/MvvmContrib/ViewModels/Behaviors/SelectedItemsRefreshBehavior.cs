namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class SelectedItemRefreshBehavior<TItemVM> :
      Behavior,
      IRefreshBehavior
      where TItemVM : IViewModel {

      public void Refresh(IBehaviorContext context, RefreshOptions options) {
         this.RefreshNext(context, options);

         TItemVM selectedItem = this.GetValueNext<TItemVM>(context);
         if (selectedItem != null) {
            selectedItem.Kernel.Refresh();
         }
      }
   }

   internal sealed class SelectedItemsRefreshBehavior<TItemVM> :
      Behavior,
      IRefreshBehavior
      where TItemVM : IViewModel {

      public void Refresh(IBehaviorContext context, RefreshOptions options) {
         this.RefreshNext(context, options);

         IVMCollection<TItemVM> selectedItems = this.GetValueNext<IVMCollection<TItemVM>>(context);
         foreach (TItemVM item in selectedItems) {
            item.Kernel.Refresh();
         }
      }
   }
}
