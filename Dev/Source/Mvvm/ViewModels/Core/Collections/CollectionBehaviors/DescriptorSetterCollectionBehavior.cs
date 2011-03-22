﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class ItemDescriptorCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>,
      IItemDescriptorProviderCollectionBehavior
      where TItemVM : IViewModel {

      private VMDescriptorBase _itemDescriptor;

      public ItemDescriptorCollectionBehavior(VMDescriptorBase itemDescriptor) {
         Contract.Requires(itemDescriptor != null);
         _itemDescriptor = itemDescriptor;
      }

      public VMDescriptorBase ItemDescriptor {
         get { return _itemDescriptor; }
      }

      public void CollectionPopulated(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         foreach (TItemVM item in collection) {
            item.Descriptor = _itemDescriptor;
         }
         this.CollectionPopulatetNext(context, collection, previousItems);
      }

      public void ItemInserted(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         item.Descriptor = _itemDescriptor;
         this.ItemInsertedNext(context, collection, item, index);
      }

      public void ItemSet(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      ) {
         item.Descriptor = _itemDescriptor;
         this.ItemSetNext(context, collection, previousItem, item, index);
      }

      public void ItemRemoved(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         this.ItemRemovedNext(context, collection, item, index);
      }

      public void CollectionCleared(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         this.ItemsClearedNext(context, collection, previousItems);
      }

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         foreach (IViewModel item in args.NewItems) {
            // TODO: Should we check old descriptor, should the Descriptor property handle this, or is it OK as it is now?
            item.Descriptor = _itemDescriptor;
         }

         this.HandleChangeNext(context, args);
      }
   }
}
