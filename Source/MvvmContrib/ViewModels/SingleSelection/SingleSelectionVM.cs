namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   public interface ISingleSelectionVM {
      IList AllItems { get; }
      object SelectedItem { get; set; }
      Type ItemSourceType { get; }
      Type ItemVMType { get; }
   }

   public abstract class SingleSelectionBaseVM<TItemSource, TItemVM> :
        ViewModel<SingleSelectionVMDescriptor<TItemSource, TItemVM>>, ISingleSelectionVM
        where TItemVM : IViewModel {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal SingleSelectionBaseVM(
         SingleSelectionVMDescriptor<TItemSource, TItemVM> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      /// <summary>
      ///   Gets or sets a filter that determines which items of the source items
      ///   should actually be returned by the <see cref="AllItems"/> property.
      ///   Items that were initially selected are always returned by the <see 
      ///   cref="AllItems"/> property.
      /// </summary>
      public Func<TItemSource, bool> ActiveItemFilter {
         get;
         set;
      }

      public IEnumerable<TItemSource> AllSourceItems {
         get { return GetValue(Descriptor.AllSourceItems); }
      }

      public TItemSource SelectedSourceItem {
         get { return GetValue(Descriptor.SelectedSourceItem); }
         set {
            if (value != null && !AllSourceItems.Contains(value)) {
               throw new ArgumentException(ExceptionTexts.SourceItemNotContainedByAllSourceItems);
            }

            SetValue(Descriptor.SelectedSourceItem, value);

            Kernel.Refresh(Descriptor.AllItems);
            Kernel.Refresh(Descriptor.SelectedItem);

         }
      }

      public IVMCollection<TItemVM> AllItems {
         get { return GetValue(Descriptor.AllItems); }
      }

      public TItemVM SelectedItem {
         get { return GetValue(Descriptor.SelectedItem); }
         set { SetValue(Descriptor.SelectedItem, value); }
      }

      /// <summary>
      ///   May contain an item that is selected, but not in the AllSourceItems collection.
      /// </summary>
      internal Optional<TItemSource> NonExistingSelectedSourceItem { get; private set; }

      IList ISingleSelectionVM.AllItems {
         get { return AllItems; }
      }

      object ISingleSelectionVM.SelectedItem {
         get { return SelectedItem; }
         set { SelectedItem = (TItemVM)value; }
      }

      Type ISingleSelectionVM.ItemSourceType {
         get { return typeof(TItemSource); }
      }

      Type ISingleSelectionVM.ItemVMType {
         get { return typeof(TItemVM); }
      }

      /// <summary>
      ///   Returns all source items for which the <see cref="ActiveItemFilter"/>
      ///   returns true or that are currently the selected item of the source object.
      ///   The selected item is always contained, even if it is not in the collection of
      ///   all items.
      /// </summary>
      internal IEnumerable<TItemSource> GetActiveSourceItems() {
         IEnumerable<TItemSource> allSourceItems = GetValue(Descriptor.AllSourceItems);
         TItemSource selectedSourceItem = GetValue(Descriptor.SelectedSourceItem);
         IEnumerable<TItemSource> activeSourceItems = null;

         if (allSourceItems == null) {
            activeSourceItems = new TItemSource[0];
         } else if (ActiveItemFilter == null) {
            activeSourceItems = allSourceItems;
         } else {
            activeSourceItems = allSourceItems
               .Where(i =>
                  ActiveItemFilter(i) ||
                  Object.Equals(selectedSourceItem, i)
               )
               .ToArray();
         }

         if (selectedSourceItem != null && !activeSourceItems.Contains(selectedSourceItem)) {
            NonExistingSelectedSourceItem = new Optional<TItemSource>(selectedSourceItem);

            activeSourceItems = activeSourceItems
               .Concat(new TItemSource[] { selectedSourceItem })
               .ToArray();
         } else {
            NonExistingSelectedSourceItem = default(Optional<TItemSource>);
         }

         return activeSourceItems;
      }
   }

   public abstract class SingleSelectionVM<TItemSource> :
      SingleSelectionBaseVM<TItemSource, SelectionItemVM<TItemSource>> {

      public SingleSelectionVM(
         SingleSelectionVMDescriptor<TItemSource> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }
   }

   public abstract class SingleSelectionVM<TItemSource, TItemVM> :
      SingleSelectionBaseVM<TItemSource, SelectableItemVM<TItemSource, TItemVM>>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public SingleSelectionVM(
         SingleSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }
   }
}