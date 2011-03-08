namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   /// <summary>
   ///   
   /// </summary>
   public class VMCollection<TItemVM> :
      BindingList<TItemVM>,
      ITypedList,
      IVMCollection<TItemVM>,
      IVMCollectionExpression<TItemVM>
      where TItemVM : IViewModel {

      private bool _isPopulating;

      /// <param name="behaviors">
      ///   The descriptor holds the collection behaviors and other metadata.
      /// </param>
      /// <param name="owner">
      ///   The view model instance that holds this collection instance. It is
      ///   the <see cref="IViewModel.Parent"/> of all items.
      /// </param>
      public VMCollection(BehaviorChain behaviors, IViewModel owner) {
         Contract.Requires<ArgumentNullException>(behaviors != null);
         Contract.Requires<ArgumentNullException>(owner != null);

         Behaviors = behaviors;
         Owner = owner;
      }

      /// <inheritdoc />
      public bool IsPopulating {
         get { return _isPopulating; }
         set {
            if (_isPopulating != value) {
               _isPopulating = value;
               OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
         }
      }

      /// <inheritdoc />
      public IViewModel Owner {
         get;
         private set;
      }

      /// <summary>
      ///   Gets the <see cref="BehaviorChain"/> of this collection.
      /// </summary>
      public BehaviorChain Behaviors {
         get;
         private set;
      }

      /// <summary>
      ///   Moves the item currently at <paramref name="fromIndex"/> to be at 
      ///   <paramref name="toIndex"/> afterwards.
      /// </summary>
      public void Move(int fromIndex, int toIndex) {
         Contract.Requires<ArgumentOutOfRangeException>(0 <= fromIndex && fromIndex < Count);
         Contract.Requires<ArgumentOutOfRangeException>(0 <= toIndex && toIndex < Count);

         var item = this[fromIndex];
         RemoveAt(fromIndex);
         Insert(toIndex, item);
      }

      /// <inheritdoc />
      protected override void OnListChanged(ListChangedEventArgs e) {
         // Do not raise ListChanged events while the list is being populated
         // to avoid endless recursions and to improve performance. A list reset
         // event is raised after population of the list.
         if (!IsPopulating) {
            base.OnListChanged(e);
         }
      }

      /// <inheritdoc />
      protected override void InsertItem(int index, TItemVM item) {
         base.InsertItem(index, item);

         if (IsPopulating) {
            return;
         }

         Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
            b.ItemInserted(Owner.GetContext(), this, item, index)
         );
      }

      /// <inheritdoc />
      protected override void RemoveItem(int index) {
         TItemVM removedItem = this[index];

         base.RemoveItem(index);

         Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
            b.ItemRemoved(Owner.GetContext(), this, removedItem, index)
         );
      }

      /// <inheritdoc />
      protected override void SetItem(int index, TItemVM item) {
         TItemVM previousItem = this[index];

         base.SetItem(index, item);

         Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
            b.ItemSet(Owner.GetContext(), this, previousItem, item, index)
         );
      }

      /// <inheritdoc />
      protected override void ClearItems() {
         TItemVM[] previousItems = this.ToArray();

         base.ClearItems();

         Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
            b.CollectionCleared(Owner.GetContext(), this, previousItems)
         );
      }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(Behaviors != null);
         Contract.Invariant(Owner != null);
      }

      public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
         var itemDescriptor = Behaviors
            .GetNextBehavior<IItemDescriptorProviderCollectionBehavior>()
            .ItemDescriptor;

         return itemDescriptor
            .Behaviors
            .GetNextBehavior<TypeDescriptorBehavior>() // TODO: Maybe define interface for it?
            .PropertyDescriptors;
      }

      public string GetListName(PropertyDescriptor[] listAccessors) {
         // This method is used only in the design-time framework and by the 
         // obsolete DataGrid control.
         return GetType().Name;
      }

      /// <inheritdoc />
      public void ReplaceItems(IEnumerable<TItemVM> newItems) {
         try {
            IsPopulating = true;
            Clear();
            newItems.ForEach(Add);
         } finally {
            IsPopulating = false;
         }
         Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
            b.CollectionPopulated(Owner.GetContext(), this)
         );
      }
   }
}
