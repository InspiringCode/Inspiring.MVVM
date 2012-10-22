namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class ChangeNotifierCollectionBehavior<TItemVM> :
      Behavior,
      ICollectionChangeHandlerBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         var c = args.Collection;

         var oldItems = (IEnumerable<IViewModel>)args.OldItems;
         var newItems = (IEnumerable<IViewModel>)args.NewItems;

         ChangeArgs a;

         switch (args.Type) {
            // No change of 'ItemsRemoved' is raised to give the client a chance
            // to distinguish if the removal was due to a repopulation (Type = 
            // 'Populated' and 'OldItems' contains items) or if the removal was
            // triggered by a 'Clear', 'Remove' or 'SetItem'.
            case CollectionChangeType.Populated:
               a = ChangeArgs.CollectionPopulated(c, oldItems, args.Reason);
               context.NotifyChange(a);
               break;
         }

         switch (args.Type) {
            case CollectionChangeType.ItemRemoved:
            case CollectionChangeType.ItemSet:
            case CollectionChangeType.ItemsCleared:
               if (args.OldItems.Any()) {
                  a = ChangeArgs.ItemsRemoved(c, oldItems, args.Reason);
                  context.NotifyChange(a);
               }
               break;
         }

         switch (args.Type) {
            case CollectionChangeType.ItemAdded:
            case CollectionChangeType.ItemSet:
               a = ChangeArgs.ItemsAdded(c, newItems, args.Reason);
               context.NotifyChange(a);
               break;
         }

         this.HandleChangeNext(context, args);
      }
   }
}
