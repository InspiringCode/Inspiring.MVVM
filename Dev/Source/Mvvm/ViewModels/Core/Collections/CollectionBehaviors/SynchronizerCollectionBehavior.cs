namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   internal sealed class SynchronizerCollectionBehavior<TItemVM, TItemSource> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public void CollectionPopulated(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection
      ) {
         this.CollectionPopulatetNext(context, collection);
      }

      public void ItemInserted(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         if (!collection.IsPopulating) {
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
         }

         this.ItemInsertedNext(context, collection, item, index);
      }

      public void ItemRemoved(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         if (!collection.IsPopulating) {
            IEnumerable<TItemSource> source = GetSource(context);

            var listSource = source as IList<TItemSource>;
            var collectionSource = source as ICollection<TItemSource>;

            if (listSource != null) {
               bool listSourceHasChanged = !Object.ReferenceEquals(listSource[index], item.Source);
               ThrowOutOfSyncExceptionIf(listSourceHasChanged);
               listSource.RemoveAt(index);
            } else if (collectionSource != null) {
               bool itemWasFound = collectionSource.Remove(item.Source);
               bool listSourceHasChanged = collectionSource.Count != collection.Count;

               ThrowOutOfSyncExceptionIf(!itemWasFound || listSourceHasChanged);
            }
         }

         this.ItemRemovedNext(context, collection, item, index);
      }

      public void ItemSet(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      ) {
         if (!collection.IsPopulating) {
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
         }

         this.ItemSetNext(context, collection, previousItem, item, index);
      }

      public void CollectionCleared(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         if (!collection.IsPopulating) {
            IEnumerable<TItemSource> source = GetSource(context);

            var collectionSource = source as ICollection<TItemSource>;
            if (collectionSource != null) {
               bool listSourceHasChanged = collectionSource.Count != previousItems.Length;
               ThrowOutOfSyncExceptionIf(listSourceHasChanged);

               collectionSource.Clear();
            }
         }

         this.ItemsClearedNext(context, collection, previousItems);
      }

      private IEnumerable<TItemSource> GetSource(IBehaviorContext context) {
         var sourceAccessorBehavior = GetNextBehavior<IValueAccessorBehavior<IEnumerable<TItemSource>>>();

         IEnumerable<TItemSource> collectionSource = sourceAccessorBehavior.GetValue(context);
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
