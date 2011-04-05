namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Linq;

   internal sealed class CollectionModificationAction<TItemVM> :
      IUndoableAction
      where TItemVM : IViewModel {

      private readonly CollectionChangedArgs<TItemVM> _change;

      public CollectionModificationAction(
          CollectionChangedArgs<TItemVM> change
      ) {
         _change = change;
      }

      public void Undo() {

         switch (_change.Type) {
            case CollectionChangeType.ItemAdded:
               _change.Collection.RemoveAt(_change.Index);
               break;
            case CollectionChangeType.ItemRemoved:
               var removedItem = _change.OldItem;
               _change.Collection.Insert(_change.Index, removedItem);
               break;
            case CollectionChangeType.ItemSet:
               var overwrittenItem = _change.OldItem;
               _change.Collection[_change.Index] = overwrittenItem;
               break;
            case CollectionChangeType.ItemsCleared:
               _change
                  .OldItems
                  .ForEach(i => _change.Collection.Add(i));
               break;
            case CollectionChangeType.Populated:
               for (int nI = 0; nI < _change.OldItems.Count(); nI++) {
                  _change.Collection[nI] = _change.OldItems.ElementAt(nI);
               }
               break;
         }
      }
   }
}
