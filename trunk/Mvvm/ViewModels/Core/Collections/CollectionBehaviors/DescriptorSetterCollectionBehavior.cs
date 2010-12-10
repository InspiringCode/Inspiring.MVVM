﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class DescriptorSetterCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      private VMDescriptorBase _itemDescriptor;

      public DescriptorSetterCollectionBehavior(VMDescriptorBase itemDescriptor) {
         Contract.Requires(itemDescriptor != null);
         _itemDescriptor = itemDescriptor;
      }

      public void ItemInserted(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM item, int index) {
         item.Descriptor = _itemDescriptor;
         this.ItemInsertedNext(context, collection, item, index);
      }

      public void ItemSet(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM previousItem, TItemVM item, int index) {
         item.Descriptor = _itemDescriptor;
         this.ItemSetNext(context, collection, previousItem, item, index);
      }

      public void ItemRemoved(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM item, int index) {
         this.ItemRemovedNext(context, collection, item, index);
      }

      public void ItemsCleared(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM[] previousItems) {
         this.ItemsClearedNext(context, collection, previousItems);
      }
   }
}