namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class ChangeNotifierCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         var c = args.Collection;

         var oldItems = (IEnumerable<IViewModel>)args.OldItems;
         var newItems = (IEnumerable<IViewModel>)args.NewItems;

         if (oldItems.Any()) {
            var a = new ChangeArgs(ChangeType.RemovedFromCollection, c, oldItems: oldItems);
            context.NotifyChange(a);
         }

         if (newItems.Any()) {
            var a = new ChangeArgs(ChangeType.AddedToCollection, c, newItems: newItems);
            context.NotifyChange(a);
         }

         this.HandleChangeNext(context, args);
      }
   }
}
