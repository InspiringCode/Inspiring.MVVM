﻿namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ChangeNotifierCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void CollectionPopulated(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         foreach (TItemVM item in collection) {
            NotifyItemAdded(item);
         }
         this.CollectionPopulatetNext(context, collection, previousItems);
      }

      public void ItemInserted(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         NotifyItemAdded(item);
         this.ItemInsertedNext(context, collection, item, index);
      }

      public void ItemRemoved(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         item.GetContext().NotifyChange(new ChangeArgs(ChangeType.RemovedFromCollection, item));
         this.ItemRemovedNext(context, collection, item, index);
      }

      public void ItemSet(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      ) {
         previousItem.GetContext().NotifyChange(new ChangeArgs(ChangeType.RemovedFromCollection, previousItem));
         item.GetContext().NotifyChange(new ChangeArgs(ChangeType.AddedToCollection, item));

         this.ItemSetNext(context, collection, previousItem, item, index);
      }

      public void CollectionCleared(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         foreach (TItemVM item in previousItems) {
            item.GetContext().NotifyChange(new ChangeArgs(ChangeType.RemovedFromCollection, item));
         }

         this.ItemsClearedNext(context, collection, previousItems);
      }

      private void NotifyItemAdded(TItemVM item) {
         item.GetContext().NotifyChange(new ChangeArgs(ChangeType.AddedToCollection, item));
      }
   }
}
