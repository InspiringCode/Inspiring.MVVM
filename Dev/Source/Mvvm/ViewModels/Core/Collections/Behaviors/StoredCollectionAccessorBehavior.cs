namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class StoredCollectionAccessorBehavior<TItemVM> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IRefreshBehavior
      where TItemVM : IViewModel {

      protected override IVMCollection<TItemVM> ProvideValue(IBehaviorContext context) {
         return this.CreateValueNext<IVMCollection<TItemVM>>(context);
      }

      public void Refresh(IBehaviorContext context) {
         var collection = GetValue(context);

         foreach (TItemVM item in collection) {
            item.Kernel.Refresh();
         }

         this.RefreshNext(context);
      }
   }
}
