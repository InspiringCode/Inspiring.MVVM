namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   internal sealed class PopulatedCollectionAccessorBehavior<TItemVM> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IRefreshBehavior
      where TItemVM : IViewModel {

      public override void SetValue(IBehaviorContext context, IVMCollection<TItemVM> value) {
         throw new NotSupportedException(
            ExceptionTexts.CannotSetVMCollectionProperties
         );
      }

      public void Refresh(IBehaviorContext context) {
         var collection = GetValue(context);
         Repopulate(context, collection);

         this.RefreshNext(context);
      }

      protected override IVMCollection<TItemVM> ProvideValue(IBehaviorContext context) {
         var collection = this.CreateValueNext<IVMCollection<TItemVM>>(context);
         Repopulate(context, collection);
         return collection;
      }

      private void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
         var sourceItems = this.GetValueNext<IEnumerable<TItemVM>>(context);
         collection.ReplaceItems(sourceItems);
      }
   }
}
