namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   internal interface ICanValidateChildrenHack {
      bool AreItemsValid(bool validateItemChildren);
   }

   public class VMCollection<TItemVM> :
      BindingList<TItemVM>,
      ITypedList,
      ISupportsValidation,
      ICanValidateChildrenHack
      where TItemVM : ViewModel {

      private IItemCreationController<TItemVM> _itemController;
      private ICollectionModificationController<TItemVM> _collectionController;
      private TItemVM _transientItem = null;
      private bool _addingItem = false;
      private ViewModel _parent;
      private CollectionValidationBehavior<TItemVM> _validationBehavior;
      private bool _suppressParentValidation = false;

      // TODO: Is public right?
      public VMCollection(
         ViewModel parent,
         VMDescriptor itemDescriptor,
         IEnumerable<TItemVM> items = null
      ) {
         _parent = parent;
         ItemDescriptor = itemDescriptor;

         if (items != null) {
            Popuplate(items);
         }
      }

      internal VMCollection(ViewModel parent, VMDescriptor itemDescriptor, CollectionValidationBehavior<TItemVM> validationBehavior) {
         _parent = parent;
         ItemDescriptor = itemDescriptor;
         _validationBehavior = validationBehavior;
      }

      public VMDescriptor ItemDescriptor { get; private set; }

      public virtual bool IsValid(bool validateChildren) {
         return validateChildren ?
            this.All(x => x.IsValid(validateChildren)) :
            true;
      }

      //HACK
      bool ICanValidateChildrenHack.AreItemsValid(bool validateItemChildren) {
         return this.All(x => x.IsValid(validateItemChildren));
      }

      public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
         return ItemDescriptor.GetService<TypeDescriptorService>().PropertyDescriptors;
      }

      public string GetListName(PropertyDescriptor[] listAccessors) {
         return GetType().Name;
      }

      internal void Revalidate() {
         foreach (TItemVM item in this) {
            item.Revalidate();
         }
      }

      public void Popuplate(
         IEnumerable<TItemVM> items,
         ICollectionModificationController<TItemVM> collectionController = null
      ) {
         Repopulate(items, null, collectionController);
      }

      internal void Repopulate(
         IEnumerable<TItemVM> items,
         IItemCreationController<TItemVM> itemController = null,
         ICollectionModificationController<TItemVM> collectionController = null
      ) {
         RaiseListChangedEvents = false;
         _suppressParentValidation = true; // HACK: To avoid Stackoverflow: Clear invokes remove, removes validates, validate gets value, validate populates, populates clears...

         var validationBehavior = _validationBehavior;
         _validationBehavior = null;

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

         _validationBehavior = validationBehavior;
         //Revalidate();

         _suppressParentValidation = false;

         RaiseListChangedEvents = true;
         OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
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
         ItemAdded(item);
         if (_collectionController != null && item != _transientItem) {
            _collectionController.Insert(item, index);
         }
      }

      protected override void RemoveItem(int index) {
         TItemVM removedItem = this[index];
         ItemRemoved(removedItem);
         base.RemoveItem(index);
         if (_collectionController != null) {
            _collectionController.Remove(removedItem);
         }
      }

      protected override void ClearItems() {
         this.ForEach(ItemRemoved);
         base.ClearItems();
         if (_collectionController != null) {
            _collectionController.Clear();
         }
      }

      protected override void SetItem(int index, TItemVM item) {
         ItemRemoved(this[index]);
         base.SetItem(index, item);
         ItemAdded(item);
         if (_collectionController != null) {
            _collectionController.SetItem(item, index);
         }
      }

      protected virtual void OnItemValidating(object sender, ValidationEventArgs args) {
         if (_validationBehavior != null) {
            _validationBehavior.Validate(this, args);
         }
      }

      protected virtual void OnItemValidated(object sender, ValidationEventArgs args) {
         if (args.AffectsOtherItems) {
            Revalidate(); // TODO: Not always necessary!
         }
      }

      protected override void OnListChanged(ListChangedEventArgs e) {
         base.OnListChanged(e);
         // TODO: Is it necessary to revalidate here? Is there any case that supports this?
      }

      // TODO: Only connect to events if required.
      private void ItemAdded(TItemVM item) {
         item.Parent = _parent;
         item.Validating += OnItemValidating;
         item.Validated += OnItemValidated;

         if (_validationBehavior != null) {
            Revalidate(); // TODO: Not always necessary!
         }

         if (!_suppressParentValidation) {
            _parent.InvokeValidate(item, null);
         }
      }

      private void ItemRemoved(TItemVM item) {
         item.Validating -= OnItemValidating;
         item.Validated -= OnItemValidated;

         if (_validationBehavior != null) {
            Revalidate(); // TODO: Not always necessary!
         }

         if (!_suppressParentValidation) {
            _parent.InvokeValidate(item, null);
         }
      }
   }

   internal interface IItemCreationController<TItemVM> {
      TItemVM AddNew();
      void EndNew(TItemVM transientItem);
      void CancelNew(TItemVM transientItem);
   }

   public interface ICollectionModificationController<TItemVM> {
      void Insert(TItemVM item, int index);
      void Remove(TItemVM item);
      void SetItem(TItemVM item, int index);
      void Clear();
   }
}
