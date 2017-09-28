namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using System.ComponentModel;
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

      /// <param name="ownerProperty">
      ///   The descriptor holds the collection behaviors and other metadata.
      /// </param>
      /// <param name="ownerVM">
      ///   The view model instance that holds this collection instance. It is
      ///   the parent of all items.
      /// </param>
      public VMCollection(IViewModel ownerVM, IVMPropertyDescriptor ownerProperty) {
         Check.NotNull(ownerVM, nameof(ownerVM));
         Check.NotNull(ownerProperty, nameof(ownerProperty));

         OwnerVM = ownerVM;
         OwnerProperty = ownerProperty;
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

      private bool IsPopulating {
         get { return _isPopulating; }
         set {
            if (_isPopulating != value) {
               _isPopulating = value;
               OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
         }
      }

      /// <summary>
      ///   Moves the item currently at <paramref name="fromIndex"/> to be at 
      ///   <paramref name="toIndex"/> afterwards.
      /// </summary>
      public void Move(int fromIndex, int toIndex) {
         Check.Requires(0 <= fromIndex && fromIndex < Count);
         Check.Requires(0 <= toIndex && toIndex < Count);

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
      }

      /// <inheritdoc />
      protected override void RemoveItem(int index) {
         TItemVM oldItem = this[index];

         base.RemoveItem(index);

         CallBehaviors(CollectionChangedArgs<TItemVM>.ItemRemoved(this, oldItem, index));
      }

      /// <inheritdoc />
      protected override void SetItem(int index, TItemVM item) {
         TItemVM oldItem = this[index];

         base.SetItem(index, item);

         CallBehaviors(CollectionChangedArgs<TItemVM>.ItemSet(this, oldItem, item, index));
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
         OwnerProperty.Behaviors.TryCall<ICollectionChangeHandlerBehavior<TItemVM>>(b =>
            b.HandleChange(OwnerVM.GetContext(), args)
         );
      }

      public string GetListName(PropertyDescriptor[] listAccessors) {
         // This method is used only in the design-time framework and by the 
         // obsolete DataGrid control.
         if (listAccessors != null && listAccessors.Any()) {
            return listAccessors
               .Last()
               .PropertyType
               .ToString();
         } else {
            return typeof(TItemVM).ToString();
         }
      }

      public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
         if (listAccessors != null && listAccessors.Any()) {
            PropertyDescriptor lastAccessor = listAccessors.Last();
            return TypeDescriptor.GetProperties(lastAccessor.PropertyType);
         }

         var itemDescriptor = this.GetItemDescriptor();
         return itemDescriptor.GetPropertyDescriptors();
      }

      /// <inheritdoc />
      void IVMCollection<TItemVM>.ReplaceItems(IEnumerable<TItemVM> newItems, IChangeReason reason) {
         TItemVM[] oldItems = this.ToArray();

         try {
            IsPopulating = true;
            Clear();
            newItems.ForEach(Add);

            // We have to call the behaviors BEFORE we raise the ListChanged event. The 
            // behavior chain does essential initialization (e.g. sets the descriptor).
            // If we raise the ListChanged before, the UI may try to access the VMs before
            // they are properly initialized.
            CallBehaviors(CollectionChangedArgs<TItemVM>.CollectionPopulated(this, oldItems, reason));
         } finally {
            IsPopulating = false;
         }
      }
   }
}
