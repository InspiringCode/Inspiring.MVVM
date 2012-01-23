namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class UndoCollectionModifcationBehavior<TItemVM> :
      Behavior,
      ICollectionChangeHandlerBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         if (!Object.Equals(args.Reason, InitialPopulationChangeReason.Instance)) {
            var manager = UndoManager.GetManager(context.VM);
            if (manager != null) {
               manager.PushAction(new CollectionModificationAction<TItemVM>(args));
            }
         }

         this.HandleChangeNext(context, args);
      }
   }
}