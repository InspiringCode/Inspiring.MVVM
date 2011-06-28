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

         if (oldItems.Any()) {
            var a = ChangeArgs.ItemsRemoved(c, oldItems);
            context.NotifyChange(a);
         }

         if (args.Type == CollectionChangeType.Populated) {
            var a = ChangeArgs.CollectionPopulated(c);
            context.NotifyChange(a);
         } else {
            if (newItems.Any()) {
               var a = ChangeArgs.ItemsAdded(c, newItems);
               context.NotifyChange(a);
            }
         }

         this.HandleChangeNext(context, args);
      }
   }
}
