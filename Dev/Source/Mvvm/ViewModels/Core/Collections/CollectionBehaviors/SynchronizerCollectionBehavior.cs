namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   internal sealed class SynchronizerCollectionBehavior<TItemVM, TItemSource> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public void CollectionPopulated(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) {
         this.CollectionPopulatetNext(context, collection, previousItems);
      }

      public void ItemInserted(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) {
         if (!collection.IsPopulating) {
            IEnumerable<TItemSource> source = GetSourceCollection(context);

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
            IEnumerable<TItemSource> source = GetSourceCollection(context);

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
            IEnumerable<TItemSource> source = GetSourceCollection(context);

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
            IEnumerable<TItemSource> source = GetSourceCollection(context);

            var collectionSource = source as ICollection<TItemSource>;
            if (collectionSource != null) {
               bool listSourceHasChanged = collectionSource.Count != previousItems.Length;
               ThrowOutOfSyncExceptionIf(listSourceHasChanged);

               collectionSource.Clear();
            }
         }

         this.ItemsClearedNext(context, collection, previousItems);
      }

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         IEnumerable<TItemSource> source = GetSourceCollection(context);
         
         switch (args.Type) {
            case CollectionChangeType.ItemAdded:
               InsertItemIntoSourceCollection(source, args);      
               break;
            case CollectionChangeType.ItemRemoved:
               RemoveItemFromSourceCollection(source, args);
               break;
            case CollectionChangeType.ItemSet:
               SetItemInSourceCollection(source, args);
               break;
            case CollectionChangeType.ItemsCleared:
               ClearSourceCollection(source, args);
               break;
         }

         this.HandleChangeNext(context, args); 
      }

      private IEnumerable<TItemSource> GetSourceCollection(IBehaviorContext context) {
         IEnumerable<TItemSource> collectionSource = this.GetValueNext<IEnumerable<TItemSource>>(context);

         Contract.Assert(collectionSource != null);
         return collectionSource;
      }


      private static void InsertItemIntoSourceCollection(IEnumerable<TItemSource> source, CollectionChangedArgs<TItemVM> args) {
         var listSource = source as IList<TItemSource>;
         var collectionSource = source as ICollection<TItemSource>;

         if (listSource != null) {
            listSource.Insert(args.Index, args.NewItem.Source);
         } else {
            if (collectionSource != null) {
               collectionSource.Add(args.NewItem.Source);
            }
         }

         if (collectionSource != null) {
            bool listSourceHasChanged = collectionSource.Count != args.Collection.Count;
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);
         }
      }

      private static void RemoveItemFromSourceCollection(IEnumerable<TItemSource> source, CollectionChangedArgs<TItemVM> args) {
         var listSource = source as IList<TItemSource>;
         var collectionSource = source as ICollection<TItemSource>;

         if (listSource != null) {
            bool listSourceHasChanged = !Object.ReferenceEquals(listSource[args.Index], args.OldItem.Source);
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);
            listSource.RemoveAt(args.Index);
         } else if (collectionSource != null) {
            bool itemWasFound = collectionSource.Remove(args.OldItem.Source);
            bool listSourceHasChanged = collectionSource.Count != args.Collection.Count;

            ThrowOutOfSyncExceptionIf(!itemWasFound || listSourceHasChanged);
         }
      }

      private static void SetItemInSourceCollection(IEnumerable<TItemSource> source, CollectionChangedArgs<TItemVM> args) {
         var listSource = source as IList<TItemSource>;
         var collectionSource = source as ICollection<TItemSource>;

         if (listSource != null) {
            bool listSourceHasChanged = !Object.Equals(listSource[args.Index], args.OldItem.Source);
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);

            listSource[args.Index] = args.NewItem.Source;
         } else {
            if (collectionSource != null) {
               bool itemWasFound = collectionSource.Remove(args.OldItem.Source);
               ThrowOutOfSyncExceptionIf(!itemWasFound);

               collectionSource.Add(args.NewItem.Source);
            }
         }

         if (collectionSource != null) {
            bool listSourceHasChanged = collectionSource.Count != args.Collection.Count;
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);
         }
      }

      private static void ClearSourceCollection(IEnumerable<TItemSource> source, CollectionChangedArgs<TItemVM> args) {
         var collectionSource = source as ICollection<TItemSource>;
         if (collectionSource != null) {
            bool listSourceHasChanged = collectionSource.Count != args.OldItems.Count();
            ThrowOutOfSyncExceptionIf(listSourceHasChanged);

            collectionSource.Clear();
         }
      }

      private static void ThrowOutOfSyncExceptionIf(bool condition) {
         if (condition) {
            throw new InvalidOperationException(ExceptionTexts.VMCollectionOutOfSync);
         }
      }
   }
}
