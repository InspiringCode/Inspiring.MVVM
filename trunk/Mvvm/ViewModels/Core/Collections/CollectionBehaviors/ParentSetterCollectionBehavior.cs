namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels.Core.BehaviorInterfaces;

   internal sealed class ParentSetterCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void ItemInserted(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         // Set the parent first so that validation and change notification can
         // propagate properly.
         item.Kernel.Parent = collection.Owner;
         this.CallNext(x => x.ItemInserted(context, collection, item, index));
      }

      public void ItemRemoved(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         // Clear the parent last so that validation and change notification can
         // propagate properly.
         this.CallNext(x => x.ItemRemoved(context, collection, item, index));
         item.Kernel.Parent = null;
      }

      public void ItemSet(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      ) {
         item.Kernel.Parent = collection.Owner;
         this.CallNext(x => x.ItemSet(context, collection, previousItem, item, index));
         previousItem.Kernel.Parent = null;
      }

      public void ItemsCleared(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         this.CallNext(x => x.ItemsCleared(context, collection, previousItems));

         foreach (TItemVM item in previousItems) {
            item.Kernel.Parent = null;
         }
      }
   }
}
