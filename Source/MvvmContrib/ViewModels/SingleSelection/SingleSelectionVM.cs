namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;

   public interface ISingleSelectionVM {
      IList AllItems { get; }
      object SelectedItem { get; set; }
      Type ItemSourceType { get; }
      Type ItemVMType { get; }
   }

   public abstract class SingleSelectionBaseVM<TItemSource, TItemVM> :
      SelectionVM<SingleSelectionVMDescriptor<TItemSource, TItemVM>, TItemSource, TItemVM>,
      ISingleSelectionVM,
      ISelectionVM
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      internal SingleSelectionBaseVM(
         SingleSelectionVMDescriptor<TItemSource, TItemVM> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
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

      IEnumerable ISelectionVM.AllSourceItems {
         get { return AllSourceItems; }
      }

      IEnumerable ISelectionVM.SelectedSourceItems {
         get {
            return SelectedSourceItem != null ?
               new[] { SelectedSourceItem } :
               null;
         }
      }

      internal TItemVM GetSelectedItem() {
         if (SelectedSourceItem == null) {
            return default(TItemVM);
         }

         return GetItemVM(SelectedSourceItem);
      }

      internal void SetSelectedItem(TItemVM selectedItem) {
         TItemSource s = selectedItem != null ?
            selectedItem.Source :
            default(TItemSource);

         // Do not use property setter because it is intended to be set by the
         // end user and executes additional code.
         SetValue(Descriptor.SelectedSourceItem, s);
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