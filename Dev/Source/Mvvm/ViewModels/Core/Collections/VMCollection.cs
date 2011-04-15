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
      public VMCollection(IViewModel ownerVM, IVMPropertyDescriptor ownerProperty) {
         Contract.Requires<ArgumentNullException>(ownerVM != null);
         Contract.Requires<ArgumentNullException>(ownerProperty != null);

         OwnerVM = ownerVM;
         OwnerProperty = ownerProperty;
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
      public IViewModel OwnerVM {
         get;
         private set;
      }

      public IVMPropertyDescriptor OwnerProperty {
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

         CallBehaviors(CollectionChangedArgs<TItemVM>.ItemInserted(this, item, index));

         //Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
         //   b.ItemInserted(Owner.GetContext(), this, item, index)
         //);
      }

      /// <inheritdoc />
      protected override void RemoveItem(int index) {
         TItemVM oldItem = this[index];

         base.RemoveItem(index);

         CallBehaviors(CollectionChangedArgs<TItemVM>.ItemRemoved(this, oldItem, index));

         //Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
         //   b.ItemRemoved(Owner.GetContext(), this, removedItem, index)
         //);
      }

      /// <inheritdoc />
      protected override void SetItem(int index, TItemVM item) {
         TItemVM oldItem = this[index];

         base.SetItem(index, item);

         CallBehaviors(CollectionChangedArgs<TItemVM>.ItemSet(this, oldItem, item, index));

         //Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
         //   b.ItemSet(Owner.GetContext(), this, previousItem, item, index)
         //);
      }

      /// <inheritdoc />
      protected override void ClearItems() {
         if (IsPopulating) {
            base.ClearItems();
            return;
         }

         TItemVM[] oldItems = this.ToArray();

         base.ClearItems();

         CallBehaviors(CollectionChangedArgs<TItemVM>.CollectionCleared(this, oldItems));
      }

      private void CallBehaviors(CollectionChangedArgs<TItemVM> args) {
         OwnerProperty.Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
            b.HandleChange(OwnerVM.GetContext(), args)
         );
      }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(OwnerProperty != null);
         Contract.Invariant(OwnerVM != null);
      }

      public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
         var itemDescriptor = this.GetItemDescriptor();

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
      void IVMCollection<TItemVM>.ReplaceItems(IEnumerable<TItemVM> newItems) {
         TItemVM[] oldItems = this.ToArray();

         try {
            IsPopulating = true;
            Clear();
            newItems.ForEach(Add);
         } finally {
            IsPopulating = false;
         }

         CallBehaviors(CollectionChangedArgs<TItemVM>.CollectionPopulated(this, oldItems));

         //Behaviors.TryCall<IModificationCollectionBehavior<TItemVM>>(b =>
         //   b.CollectionPopulated(Owner.GetContext(), this, previousItems)
         //);
      }
   }
}
