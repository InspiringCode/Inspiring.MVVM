namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   internal sealed class PopulatedCollectionAccessorBehavior<TItemVM> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IRefreshBehavior
      where TItemVM : IViewModel {

      protected override IVMCollection<TItemVM> ProvideValue(IBehaviorContext context) {
         var collection = this.CreateValueNext<IVMCollection<TItemVM>>(context);
         Repopulate(context, collection);
         return collection;
      }

      public void Refresh(IBehaviorContext context) {
         var collection = GetValue(context);
         Repopulate(context, collection);

         this.RefreshNext(context);
      }

      private void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
         var sourceItems = this.GetValueNext<IEnumerable<TItemVM>>(context);
         collection.ReplaceItems(sourceItems);
      }
   }
}
