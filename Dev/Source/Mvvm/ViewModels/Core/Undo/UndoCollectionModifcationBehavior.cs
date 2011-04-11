namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class UndoCollectionModifcationBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         var manager = UndoManager.GetManager(context.VM);
         if (manager != null) {
            manager.PushAction(new CollectionModificationAction<TItemVM>(args));
         }
         this.HandleChangeNext(context, args);
      }
   }
}