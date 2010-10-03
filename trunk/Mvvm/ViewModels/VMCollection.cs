namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   public class VMCollection<TItemVM> :
      BindingList<TItemVM>,
      ITypedList,
      ISupportsValidation
      where TItemVM : ViewModel {

      private IItemCreationController<TItemVM> _itemController;
      private ICollectionModificationController<TItemVM> _collectionController;
      private TItemVM _transientItem = null;
      private bool _addingItem = false;
      private ViewModel _parent;

      internal VMCollection(ViewModel parent, VMDescriptor itemDescriptor) {
         _parent = parent;
         ItemDescriptor = itemDescriptor;
      }

      public VMDescriptor ItemDescriptor { get; private set; }

      public virtual bool IsValid(bool validateChildren) {
         return validateChildren ?
            this.All(x => x.IsValid(validateChildren)) :
            true;
      }

      public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
         return ItemDescriptor.PropertyDescriptors;
      }

      public string GetListName(PropertyDescriptor[] listAccessors) {
         return GetType().Name;
      }

      internal void Repopulate(
         IEnumerable<TItemVM> items,
         IItemCreationController<TItemVM> itemController = null,
         ICollectionModificationController<TItemVM> collectionController = null
      ) {
         // Disable the controllers while populating
         _itemController = null;
         _collectionController = null;

         Clear();

         // TODO: Make this more efficient
         items.ForEach(Add);

         AllowNew = itemController != null;
         AllowRemove = collectionController != null;

         _itemController = itemController;
         _collectionController = collectionController;
      }

      protected override void OnAddingNew(AddingNewEventArgs e) {
         base.OnAddingNew(e);

         // We this here and not in 'AddNewCore' because the 'AddNewCore' handles
         // the addition of new item correctly if we do it this way.
         if (_itemController != null && e.NewObject == null) {
            // We set _transientItem here and _addingItem in the AddNewCore method
            // to prevent that ICollectionModificationController.Insert is called
            // for the transient item.
            _transientItem = _itemController.AddNew();
            e.NewObject = _transientItem;
         }
      }

      protected override object AddNewCore() {
         var item = (TItemVM)base.AddNewCore();
         _addingItem = true;
         return item;
      }

      public override void EndNew(int itemIndex) {
         base.EndNew(itemIndex);

         if (_itemController != null &&
             _addingItem &&
             IndexOf(_transientItem) == itemIndex
         ) {
            _itemController.EndNew(_transientItem);
            _transientItem = null;
            _addingItem = false;
         }
      }

      protected override void InsertItem(int index, TItemVM item) {
         base.InsertItem(index, item);
         item.Parent = _parent;
         if (_collectionController != null && item != _transientItem) {
            _collectionController.Insert(item, index);
         }
      }

      protected override void RemoveItem(int index) {
         TItemVM removedItem = this[index];
         base.RemoveItem(index);
         if (_collectionController != null) {
            _collectionController.Remove(removedItem);
         }
      }

      protected override void ClearItems() {
         base.ClearItems();
         if (_collectionController != null) {
            _collectionController.Clear();
         }
      }

      protected override void SetItem(int index, TItemVM item) {
         base.SetItem(index, item);
         item.Parent = _parent;
         if (_collectionController != null) {
            _collectionController.SetItem(item, index);
         }
      }
   }

   internal interface IItemCreationController<TItemVM> {
      TItemVM AddNew();
      void EndNew(TItemVM transientItem);
      void CancelNew(TItemVM transientItem);
   }

   internal interface ICollectionModificationController<TItemVM> {
      void Insert(TItemVM item, int index);
      void Remove(TItemVM item);
      void SetItem(TItemVM item, int index);
      void Clear();
   }
}
