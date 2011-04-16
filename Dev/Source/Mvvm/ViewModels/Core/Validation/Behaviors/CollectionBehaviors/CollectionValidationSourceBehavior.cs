namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   internal sealed class CollectionValidationSourceBehavior<TItemVM> :
      Behavior,
      ICollectionChangeHandlerBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         switch (args.Type) {
            case CollectionChangeType.ItemAdded:
               ValidateItem(args.NewItem);
               break;
            case CollectionChangeType.ItemRemoved:
               ValidateItem(args.OldItem);
               break;
            case CollectionChangeType.ItemSet:
               ValidateItem(args.OldItem);
               ValidateItem(args.NewItem);
               break;
            case CollectionChangeType.ItemsCleared:
               ValidateItems(args.OldItems);
               break;
            case CollectionChangeType.Populated:
               // We do not validate when the collection is populated because this is
               // done by the DescendantsValidatorBehavior.
               break;
            default:
               throw new NotSupportedException();
         }

         this.HandleChangeNext(context, args);
      }

      private void ValidateItem(IViewModel item) {
         Revalidator.Revalidate(item, ValidationScope.SelfAndLoadedDescendants);
      }

      private void ValidateItems(IEnumerable<TItemVM> items) {
         var enumerableItems = (IEnumerable<IViewModel>)items;

         Revalidator.RevalidateItems(
            enumerableItems,
            ValidationScope.SelfAndLoadedDescendants
         );
      }
   }
}
