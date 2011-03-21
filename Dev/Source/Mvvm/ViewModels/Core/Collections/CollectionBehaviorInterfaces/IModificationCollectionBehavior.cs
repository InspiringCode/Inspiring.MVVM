namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Contracts;

   /// <summary>
   ///   A collection behavior whose methods are called by the <see cref="VMCollection"/>
   ///   if it is modified in some way. Implement this interface if you want to do
   ///   something when items are inserted, removed or replaced.
   /// </summary>
   /// <remarks>
   ///   Note that all the behavior method are called after their causing VM collection
   ///   methods. This is because some behaviors require that the collection was already
   ///   modified when the behavior methods are called. For example a validation behavior
   ///   may invoke validations which access the current state of the collection.
   /// </remarks>
   [ContractClass(typeof(ICollectionModificationBehaviorContracts<>))]
   public interface IModificationCollectionBehavior<TItemVM> : IBehavior where TItemVM : IViewModel {

      /// <summary>
      /// Called after the <paramref name="collection"/> is populated.
      /// </summary>
      void CollectionPopulated(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      );

      /// <summary>
      ///   Called after the <paramref name="item"/> is added/inserted into the 
      ///   <paramref name="collection"/>.
      /// </summary>
      void ItemInserted(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      );

      /// <summary>
      ///   Called after the <paramref name="item"/> is removed from the <paramref 
      ///   name="collection"/>.
      /// </summary>
      void ItemRemoved(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      );

      /// <summary>
      ///   Called after the item at the passed <paramref name="index"/> is replaced
      ///   with the passed <paramref name="item"/> in the <paramref name="collection"/>.
      /// </summary>
      void ItemSet(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      );

      /// <summary>
      ///   Called after all items of the <paramref name="collection"/> have been
      ///   removed.
      /// </summary>
      void CollectionCleared(
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      );

      //void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {

      //}
   }

   public enum CollectionChangeType {
      ItemAdded,
      ItemRemoved,
      ItemSet,
      ItemsCleared,
      Populated
   }

   public class CollectionChangedArgs<TItemVM> {
      private CollectionChangedArgs(
         CollectionChangeType type,
         IVMCollection<TItemVM> collection,
         int index,
         TItemVM oldItem = default(TItemVM),
         TItemVM newItem = default(TItemVM)
      ) {
         Type = type;
         Collection = collection;
         OldItem = oldItem;
         OldItems = new[] { oldItem };
         NewItem = newItem;
         NewItems = new[] { newItem };
      }

      private CollectionChangedArgs(
         CollectionChangeType type,
         IVMCollection<TItemVM> collection,
         IEnumerable<TItemVM> oldItems
      ) {
         Type = type;
         Collection = collection;
         OldItems = collection;
         NewItems = oldItems;
      }

      public CollectionChangeType Type { get; private set; }

      public IVMCollection Collection { get; private set; }

      public TItemVM OldItem { get; private set; }

      public TItemVM NewItem { get; private set; }

      public IEnumerable<TItemVM> OldItems { get; private set; }

      public IEnumerable<TItemVM> NewItems { get; private set; }

      public static CollectionChangedArgs<TItemVM> ItemAdded(
         IVMCollection<TItemVM> collection,
         TItemVM newItem,
         int index
      ) {
         Contract.Requires(collection != null);
         Contract.Requires(newItem != null);
         Contract.Requires(0 <= index && index <= collection.Count);

         return new CollectionChangedArgs<TItemVM>(
            CollectionChangeType.ItemAdded,
            collection,
            index,
            newItem: newItem
         );
      }

      public static CollectionChangedArgs<TItemVM> ItemRemoved(
         IVMCollection<TItemVM> collection,
         TItemVM oldItem,
         int index
      ) {
         Contract.Requires(collection != null);
         Contract.Requires(oldItem != null);
         Contract.Requires(0 <= index && index <= collection.Count);

         return new CollectionChangedArgs<TItemVM>(
            CollectionChangeType.ItemRemoved,
            collection,
            index,
            oldItem: oldItem
         );
      }

      public static CollectionChangedArgs<TItemVM> ItemSet(
         IVMCollection<TItemVM> collection,
         TItemVM oldItem,
         TItemVM newItem,
         int index
      ) {
         Contract.Requires(collection != null);
         Contract.Requires(oldItem != null);
         Contract.Requires(newItem != null);
         Contract.Requires(0 <= index && index < collection.Count);

         return new CollectionChangedArgs<TItemVM>(
            CollectionChangeType.ItemSet,
            collection,
            index,
            oldItem: oldItem,
            newItem: newItem
         );
      }

      public static CollectionChangedArgs<TItemVM> CollectionCleared(
         IVMCollection<TItemVM> collection,
         TItemVM[] oldItems
      ) {
         Contract.Requires(collection != null);
         Contract.Requires(oldItems != null);

         return new CollectionChangedArgs<TItemVM>(
            CollectionChangeType.ItemsCleared,
            collection,
            oldItems
         );
      }

      public static CollectionChangedArgs<TItemVM> CollectionPopulated(
         IVMCollection<TItemVM> collection,
         TItemVM[] oldItems,
         int index
      ) {
         Contract.Requires(collection != null);

         return new CollectionChangedArgs<TItemVM>(
            CollectionChangeType.Populated,
            collection,
            oldItems
         );
      }
   }

   namespace Contracts {
      [ContractClassFor(typeof(IModificationCollectionBehavior<>))]
      internal abstract class ICollectionModificationBehaviorContracts<TItemVM> :
         IModificationCollectionBehavior<TItemVM>
         where TItemVM : IViewModel {

         public void CollectionPopulated(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM[] previousItems) {
            Contract.Requires(context != null);
            Contract.Requires(collection != null);
         }

         public void ItemInserted(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM item, int index) {
            Contract.Requires(context != null);
            Contract.Requires(collection != null);
            Contract.Requires(item != null);
            Contract.Requires(0 <= index && index <= collection.Count);
         }

         public void ItemRemoved(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM item, int index) {
            Contract.Requires(context != null);
            Contract.Requires(collection != null);
            Contract.Requires(item != null);
            Contract.Requires(0 <= index && index <= collection.Count);
         }

         public void ItemSet(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM previousItem, TItemVM item, int index) {
            Contract.Requires(context != null);
            Contract.Requires(collection != null);
            Contract.Requires(previousItem != null);
            Contract.Requires(item != null);
            Contract.Requires(0 <= index && index < collection.Count);
         }

         public void CollectionCleared(IBehaviorContext context, IVMCollection<TItemVM> collection, TItemVM[] previousItems) {
            Contract.Requires(context != null);
            Contract.Requires(collection != null);
            Contract.Requires(previousItems != null);
         }

         public abstract IBehavior Successor { get; set; }

         public void Initialize(BehaviorInitializationContext context) {
            // TODO: Remove
         }
      }
   }
}
