namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   // TODO: Test
   internal sealed class SelectableItemsCollection<T> : ICollection<T> {
      private readonly IEnumerable<T> _allSourceItems;
      private readonly HashSet<T> _items;

      public SelectableItemsCollection(
         IEnumerable<T> allSourceItems,
         IEnumerable<T> selectedItems,
         Func<T, bool> isActiveFilter
      ) {
         IEnumerable<T> selectableItems = allSourceItems;

         if (isActiveFilter != null) {
            selectableItems = selectableItems.Where(isActiveFilter);
         }

         // Add items that are either inactive but currently selected or
         // are not contained in the all items colletion at all.
         if (selectedItems != null) {
            // We can use Concat (which is faster) because we create a HashSet
            // from the items.
            selectableItems = selectableItems.Concat(selectedItems);
         }

         _items = new HashSet<T>(selectableItems);
         _allSourceItems = allSourceItems;
      }

      public int Count {
         get { return _items.Count; }
      }

      public bool IsReadOnly {
         get {
            ICollection<T> editable = _items as ICollection<T>;
            return editable == null || editable.IsReadOnly;
         }
      }

      private ICollection<T> EditableSourceItems {
         get {
            var editable = _allSourceItems as ICollection<T>;

            if (editable == null) {
               throw new InvalidOperationException(); // TODO: Exception text.
            }

            return editable;
         }
      }

      public void Add(T item) {
         _items.Add(item);
         EditableSourceItems.Add(item);
      }

      public void Clear() {
         _items.Clear();
         EditableSourceItems.Clear();
      }

      public bool Contains(T item) {
         return _items.Contains(item);
      }

      public void CopyTo(T[] array, int arrayIndex) {
         _items.CopyTo(array, arrayIndex);
      }

      public bool Remove(T item) {
         bool result = _items.Remove(item);
         EditableSourceItems.Remove(item);
         return result;
      }

      public IEnumerator<T> GetEnumerator() {
         return _items.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return _items.GetEnumerator();
      }
   }

   internal sealed class ItemProviderBehavior<TItemSource> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<IEnumerable<TItemSource>>,
      IRefreshControllerBehavior {

      private static readonly FieldDefinitionGroup ItemsGroup = new FieldDefinitionGroup();
      //private readonly Func<IViewModel, IEnumerable<TItemSource>> _selectedItemsSelector;
      //private readonly IVMPropertyDescriptor<IEnumerable<TItemSource>> _allItemsProperty;
      private FieldDefinition<SelectableItemsCollection<TItemSource>> _items;

      //public ItemProviderBehavior(
      //   IVMPropertyDescriptor<IEnumerable<TItemSource>> allItemsProperty,
      //   IVMPropertyDescriptor<ICollection<TItemSource>> selectedSourceItemsProperty
      //) {
      //   _allItemsProperty = allItemsProperty;
      //   _selectedItemsSelector = (vm) =>
      //      vm.Kernel.GetValue(selectedSourceItemsProperty);
      //}

      //public ItemProviderBehavior(
      //   IVMPropertyDescriptor<IEnumerable<TItemSource>> allItemsProperty,
      //   IVMPropertyDescriptor<TItemSource> selectedSourceItemProperty
      //) {
      //   _allItemsProperty = allItemsProperty;
      //   _selectedItemsSelector = (vm) => {
      //      TItemSource selected = vm.Kernel.GetValue(selectedSourceItemProperty);

      //      return Object.Equals(selected, null) ?
      //         new TItemSource[0] :
      //         new[] { selected };
      //   };
      //}

      public Func<TItemSource, bool> IsActiveFilter {
         get;
         set;
      }

      public void Initialize(BehaviorInitializationContext context) {
         _items = context.Fields.DefineField<SelectableItemsCollection<TItemSource>>(ItemsGroup);
         this.InitializeNext(context);
      }

      public SelectableItemsCollection<TItemSource> GetSelectableItems(IBehaviorContext context) {
         SelectableItemsCollection<TItemSource> items;

         if (!context.FieldValues.TryGetValue(_items, out items)) {
            items = new SelectableItemsCollection<TItemSource>(
               SelectionHelpers.GetAllSourceItems<TItemSource>(context.VM),
               SelectionHelpers.GetSelectedSourceItems<TItemSource>(context.VM),
               IsActiveFilter
            );

            context.FieldValues.SetValue(_items, items);
         }

         return items;
      }

      public IEnumerable<TItemSource> GetValue(IBehaviorContext context) {
         return GetSelectableItems(context);
      }

      public void SetValue(IBehaviorContext context, IEnumerable<TItemSource> value) {
         this.SetValueNext(context, value);
      }

      public void Refresh(IBehaviorContext context) {
         this.ViewModelRefreshNext(context);
      }

      //private SourceItemsCache CreateCache(IViewModel selectionVM) {
      //   IEnumerable<TItemSource> allItems = SelectionHelpers
      //      .GetAllSourceItems<TItemSource>(selectionVM);

      //   IEnumerable<TItemSource> selectedItems = SelectionHelpers
      //      .GetSelectedSourceItems<TItemSource>(selectionVM);

      //   IEnumerable<TItemSource> selectableItems = allItems;

      //   if (IsActiveFilter != null) {
      //      selectableItems = selectableItems.Where(IsActiveFilter);
      //   }

      //   if (selectedItems != null && selectedItems.Any()) {
      //      // Add items that are either inactive but currently selected or
      //      // are not contained in the all items collection at all.
      //      selectableItems = selectableItems.Union(selectedItems);
      //   }

      //   return new SourceItemsCache {
      //      SelectableSourceItems = selectableItems.ToList()
      //   };
      //}

      //private class SourceItemsCache {
      //   //public IEnumerable<TItemSource> AllSourceItems { get; set; }
      //   public IEnumerable<TItemSource> SelectableSourceItems { get; set; }
      //}

      public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property) {
         context.FieldValues.ClearField(_items);
         this.ViewModelRefreshNext(context, property);
      }
   }

   internal class SourceItemCollections<TItemSource> {
      internal SourceItemCollections(
         IViewModel selectionVM,
         Func<TItemSource, bool> isActiveFilter
      ) {
         IEnumerable<TItemSource> allItems = SelectionHelpers
            .GetAllSourceItems<TItemSource>(selectionVM);

         IEnumerable<TItemSource> selectedItems = SelectionHelpers
            .GetSelectedSourceItems<TItemSource>(selectionVM);

         AllSourceItems = new HashSet<TItemSource>(allItems);

         IEnumerable<TItemSource> selectableItems = allItems;

         if (isActiveFilter != null) {
            selectableItems = selectableItems.Where(isActiveFilter);
         }

         if (selectedItems != null && selectedItems.Any()) {
            // Add items that are either inactive but currently selected or do
            // are not contained in the all items at all.
            selectableItems = selectableItems.Union(selectedItems);
         }

         SelectableItems = selectableItems.ToList();
      }

      public IEnumerable<TItemSource> AllSourceItems { get; private set; }
      public IEnumerable<TItemSource> SelectableItems { get; private set; }

      //public IEnumerable<TItemSource> SelectedItems { get; private set; }

      //public ISet<TItemSource> SelectedItemsNotContainedInSource {
      //   get {
      //      var selectedItems = _selectedItemsProvider();

      //      return new HashSet<TItemSource>(
      //         selectedItems.Where(x => !AllSourceItems.Contains(x))
      //      );
      //   }
      //}

      //public bool IsItemContainedInSource(TItemSource item) {
      //   return AllSourceItems.Contains(item);
      //}
   }
}
