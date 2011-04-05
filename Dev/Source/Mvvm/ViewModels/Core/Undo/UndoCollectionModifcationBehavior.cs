namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class UndoCollectionModifcationBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void CollectionPopulated(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM[] previousItems) {
         this.CollectionPopulatetNext(context, collection, previousItems);
      }

      public void ItemInserted(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM item, int index) {
         this.ItemInsertedNext(context, collection, item, index);
      }

      public void ItemRemoved(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM item, int index) {
         this.ItemRemovedNext(context, collection, item, index);
      }

      public void ItemSet(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM previousItem, TItemVM item, int index) {
         this.ItemSetNext(context, collection, previousItem, item, index);
      }

      public void CollectionCleared(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM[] previousItems) {
         this.ItemsClearedNext(context, collection, previousItems);
      }

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         UndoManager
            .GetManager(context.VM)
            .PushAction(new CollectionModificationAction<TItemVM>(args));
         this.HandleChangeNext(context, args);
      }


   }
}