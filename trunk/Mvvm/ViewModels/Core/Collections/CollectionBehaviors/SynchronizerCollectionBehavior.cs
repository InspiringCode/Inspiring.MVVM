namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core.BehaviorInterfaces;

   internal sealed class SynchronizerCollectionBehavior<TItemVM, TItemSource> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

      public void ItemInserted(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         IEnumerable<TItemSource> source = GetSource(context);

         var listSource = source as IList<TItemSource>;
         var collectionSource = source as ICollection<TItemSource>;

         if (listSource != null) {
            listSource.Insert(index, item.Source);
         } else {
            if (collectionSource != null) {
               collectionSource.Add(item.Source);
            }
         }

         if (collectionSource != null) {
            bool listSourceHasChanged = collectionSource.Count != collection.Count;
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);
         }


         this.CallNext(x => x.ItemInserted(context, collection, item, index));
      }

      public void ItemRemoved(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         IEnumerable<TItemSource> source = GetSource(context);

         var listSource = source as IList<TItemSource>;
         var collectionSource = source as ICollection<TItemSource>;

         if (listSource != null) {
            bool listSourceHasChanged = !Object.ReferenceEquals(listSource[index], item.Source);
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);
         }

         if (collectionSource != null) {
            bool itemWasFound = collectionSource.Remove(item.Source);
            bool listSourceHasChanged = collectionSource.Count != collection.Count;

            ThrowOutOfSyncExceptionIf(!itemWasFound || listSourceHasChanged);
         }

         this.CallNext(x => x.ItemRemoved(context, collection, item, index));
      }

      public void ItemSet(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      ) {
         IEnumerable<TItemSource> source = GetSource(context);

         var listSource = source as IList<TItemSource>;
         var collectionSource = source as ICollection<TItemSource>;

         if (listSource != null) {
            bool listSourceHasChanged = !Object.Equals(listSource[index], previousItem.Source);
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);

            listSource[index] = item.Source;
         } else {
            if (collectionSource != null) {
               bool itemWasFound = collectionSource.Remove(previousItem.Source);
               ThrowOutOfSyncExceptionIf(!itemWasFound);

               collectionSource.Add(item.Source);
            }
         }

         if (collectionSource != null) {
            bool listSourceHasChanged = collectionSource.Count != collection.Count;
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);
         }

         this.CallNext(x => x.ItemSet(context, collection, previousItem, item, index));
      }

      public void ItemsCleared(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         IEnumerable<TItemSource> source = GetSource(context);

         var collectionSource = source as ICollection<TItemSource>;
         if (collectionSource != null) {
            bool listSourceHasChanged = collectionSource.Count != previousItems.Length;
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);

            collectionSource.Clear();
         }

         this.CallNext(x => x.ItemsCleared(context, collection, previousItems));
      }

      private IEnumerable<TItemSource> GetSource(IBehaviorContext context) {
         var sourceAccessorBehavior = GetNextBehavior<IValueAccessorBehavior<IEnumerable<TItemSource>>>();

         IEnumerable<TItemSource> collectionSource = sourceAccessorBehavior.GetValue(context, ValueStage.PostValidation);
         Contract.Assert(collectionSource != null);

         return collectionSource;
      }

      private void ThrowOutOfSyncExceptionIf(bool condition) {
         if (condition) {
            throw new InvalidOperationException(ExceptionTexts.VMCollectionOutOfSync);
         }
      }
   }
}
