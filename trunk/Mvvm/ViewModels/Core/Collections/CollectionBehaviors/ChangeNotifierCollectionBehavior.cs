namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels.Core.BehaviorInterfaces;

   internal sealed class ChangeNotifierCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void ItemInserted(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         context.NotifyChange(new ChangeArgs(ChangeType.AddedToCollection, item));
         this.CallNext(x => x.ItemInserted(context, collection, item, index));
      }

      public void ItemRemoved(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         context.NotifyChange(new ChangeArgs(ChangeType.RemovedFromCollection, item));
         this.CallNext(x => x.ItemRemoved(context, collection, item, index));
      }

      public void ItemSet(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      ) {
         context.NotifyChange(new ChangeArgs(ChangeType.RemovedFromCollection, previousItem));
         context.NotifyChange(new ChangeArgs(ChangeType.AddedToCollection, item));

         this.CallNext(x => x.ItemSet(context, collection, previousItem, item, index));
      }

      public void ItemsCleared(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         foreach (TItemVM item in previousItems) {
            context.NotifyChange(new ChangeArgs(ChangeType.RemovedFromCollection, item));
         }

         this.CallNext(x => x.ItemsCleared(context, collection, previousItems));
      }
   }
}
