namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ParentSetterCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void CollectionPopulated(
        IBehaviorContext context,
        IVMCollection<TItemVM> collection
      ) {
         foreach (TItemVM item in collection) {
            HandleItemInserted(item, collection);
         }
         this.CollectionPopulatetNext(context, collection);
      }

      public void ItemInserted(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         HandleItemInserted(item, collection);
         this.ItemInsertedNext(context, collection, item, index);
      }

      public void ItemRemoved(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         this.ItemRemovedNext(context, collection, item, index);
         HandleItemRemoved(item, collection);
      }

      public void ItemSet(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      ) {
         HandleItemInserted(item, collection);
         this.ItemSetNext(context, collection, previousItem, item, index);
         HandleItemRemoved(previousItem, collection);
      }

      public void CollectionCleared(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         this.ItemsClearedNext(context, collection, previousItems);

         foreach (TItemVM item in previousItems) {
            HandleItemRemoved(item, collection);
         }
      }

      private void HandleItemInserted(TItemVM item, IVMCollection<TItemVM> collection) {
         if (item.Kernel.OwnerCollection == null) {
            // Set the parent first so that validation and change notification can
            // propagate properly.
            item.Kernel.Parent = collection.Owner;
            item.Kernel.OwnerCollection = collection;
         }
      }

      private void HandleItemRemoved(TItemVM item, IVMCollection<TItemVM> collection) {
         if (item.Kernel.OwnerCollection == collection) {
            // Clear the parent last so that validation and change notification can
            // propagate properly.
            item.Kernel.Parent = null;
            item.Kernel.OwnerCollection = null;
         }
      }
   }
}
