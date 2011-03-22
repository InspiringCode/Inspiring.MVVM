namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ParentSetterCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void CollectionPopulated(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         foreach (TItemVM item in previousItems) {
            HandleItemRemoved(item, collection);
         }

         foreach (TItemVM item in collection) {
            HandleItemInserted(item, collection);
         }

         this.CollectionPopulatetNext(context, collection, previousItems);
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
         item.Kernel.Parent = collection.Owner;
         item.Kernel.OwnerCollections.Add(collection);
      }

      private void HandleItemRemoved(TItemVM item, IVMCollection<TItemVM> collection) {
         item.Kernel.Parent = null;
         item.Kernel.OwnerCollections.Remove(collection);
      }


      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         // Set the parent first so that validation and change notification can
         // propagate properly.
         foreach (IViewModel item in args.NewItems) {
            item.Kernel.Parent = args.Collection.Owner;
            item.Kernel.OwnerCollections.Add(args.Collection);
         }

         this.HandleChangeNext(context, args);

         // Clear the parent last so that validation and change notification can
         // propagate properly.
         foreach (IViewModel item in args.OldItems) {
            item.Kernel.Parent = null;
            item.Kernel.OwnerCollections.Remove(args.Collection);
         }
      }
   }
}
