namespace Inspiring.Mvvm.ViewModels.Core.Validation.CollectionBehaviors {
   using System.Collections.Generic;

   internal sealed class CollectionValidationSourceBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void CollectionPopulated(IBehaviorContext context, IVMCollection<TItemVM> collection) {
         // TODO: What is the best way to handle initial/repopulation handling????

         this.CollectionPopulatetNext(context, collection);
      }

      public void ItemInserted(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM item, int index) {
         ValidateItem(item);

         this.ItemInsertedNext(context, collection, item, index);
      }

      public void ItemRemoved(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM item, int index) {
         ValidateItem(item);

         this.ItemRemovedNext(context, collection, item, index);
      }

      public void ItemSet(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM previousItem, TItemVM item, int index) {
         ValidateItem(previousItem);
         ValidateItem(item);

         this.ItemSetNext(context, collection, previousItem, item, index);
      }

      public void CollectionCleared(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM[] previousItems) {
         ValidateItems(previousItems);

         this.ItemsClearedNext(context, collection, previousItems);
      }

      private void ValidateItem(IViewModel item) {
         // TODO: With what scope/when should here be revalidated?
         new HierarchicalRevalidator(item, ValidationScope.SelfAndLoadedDescendants)
            .Revalidate();
      }

      private void ValidateItems(IEnumerable<TItemVM> items) {
         new HierarchicalRevalidator(null, ValidationScope.SelfAndLoadedDescendants)
            .RevalidateCollection((IEnumerable<IViewModel>)items); // TODO: Why do we need to cast here?
      }
   }
}
