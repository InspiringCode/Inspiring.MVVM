namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class ItemProviderBehavior<TItemSource> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<IEnumerable<TItemSource>>,
      IRefreshBehavior {

      private static readonly FieldDefinitionGroup ItemsGroup = new FieldDefinitionGroup();
      private readonly Func<IViewModel, IEnumerable<TItemSource>> _selectedItemsSelector;
      private readonly IVMPropertyDescriptor<IEnumerable<TItemSource>> _allItemsProperty;
      private FieldDefinition<SourceItemCollections<TItemSource>> _items;

      public ItemProviderBehavior(
         IVMPropertyDescriptor<IEnumerable<TItemSource>> allItemsProperty,
         IVMPropertyDescriptor<ICollection<TItemSource>> selectedSourceItemsProperty
      ) {
         _allItemsProperty = allItemsProperty;
         _selectedItemsSelector = (vm) =>
            vm.Kernel.GetValue(selectedSourceItemsProperty);
      }

      public ItemProviderBehavior(
         IVMPropertyDescriptor<IEnumerable<TItemSource>> allItemsProperty,
         IVMPropertyDescriptor<TItemSource> selectedSourceItemProperty
      ) {
         _allItemsProperty = allItemsProperty;
         _selectedItemsSelector = (vm) => {
            TItemSource selected = vm.Kernel.GetValue(selectedSourceItemProperty);

            return Object.Equals(selected, null) ?
               new TItemSource[0] :
               new[] { selected };
         };
      }

      public Func<TItemSource, bool> IsActiveFilter {
         get;
         set;
      }

      public void Initialize(BehaviorInitializationContext context) {
         _items = context.Fields.DefineField<SourceItemCollections<TItemSource>>(ItemsGroup);
         this.InitializeNext(context);
      }

      public SourceItemCollections<TItemSource> GetCollections(IBehaviorContext context) {
         SourceItemCollections<TItemSource> items;

         if (!context.FieldValues.TryGetValue(_items, out items)) {
            var vm = context.VM;
            IEnumerable<TItemSource> allItems = vm.Kernel.GetValue(_allItemsProperty);
            IEnumerable<TItemSource> selectedItems = _selectedItemsSelector(vm);

            items = new SourceItemCollections<TItemSource>(allItems, selectedItems, IsActiveFilter);
            context.FieldValues.SetValue(_items, items);
         }

         return items;
      }

      public IEnumerable<TItemSource> GetValue(IBehaviorContext context) {
         return GetCollections(context).SelectableItems;
      }

      public void SetValue(IBehaviorContext context, IEnumerable<TItemSource> value) {
         this.SetValueNext(context, value);
      }

      public void Refresh(IBehaviorContext context) {
         context.FieldValues.ClearField(_items);
         this.RefreshNext(context);
      }
   }

   internal class SourceItemCollections<TItemSource> {
      internal SourceItemCollections(
         IEnumerable<TItemSource> allItems,
         IEnumerable<TItemSource> selectedItems,
         Func<TItemSource, bool> isActiveFilter
      ) {
         allItems = allItems ?? Enumerable.Empty<TItemSource>();

         SelectedItems = selectedItems;
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

      public ISet<TItemSource> AllSourceItems { get; private set; }
      public IEnumerable<TItemSource> SelectableItems { get; private set; }
      public IEnumerable<TItemSource> SelectedItems { get; private set; }

      public ISet<TItemSource> SelectedItemsNotContainedInSource {
         get {
            return new HashSet<TItemSource>(
               SelectedItems.Where(x => !AllSourceItems.Contains(x))
            );
         }
      }
   }
}
