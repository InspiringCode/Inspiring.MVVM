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

      public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
         var collection = GetValue(context);
         Repopulate(context, collection, RefreshReason.Create(executeRefreshDependencies));

         this.RefreshNext(context, executeRefreshDependencies);
      }

      protected override IVMCollection<TItemVM> ProvideValue(IBehaviorContext context) {
         return this.CreateValueNext<IVMCollection<TItemVM>>(context);
      }

      protected override void OnInitialize(IBehaviorContext context) {
         // We have to populate the collection here and NOT in the ProvideValue
         // method to avoid the following endless recursion:
         //   1. GetValue call ProvideValue.
         //   2. ProvideValue populates the collection, which raises a change event.
         //   3. The change event triggers a view model level validation.
         //   4. The view model level validation may access the property again
         //      which calls GetValue and ProvideValue.

         var collection = GetValue(context);
         Repopulate(context, collection, reason: null);

         base.OnInitialize(context);
      }

      private void Repopulate(
         IBehaviorContext context, 
         IVMCollection<TItemVM> collection, 
         IChangeReason reason
      ) {
         var sourceItems = this.GetValueNext<IEnumerable<TItemVM>>(context);
         collection.ReplaceItems(sourceItems, reason);
      }
   }
}
