namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   public sealed class SingleSelectionVM<TSourceItem, TItemVM> :
      ViewModel<SingleSelectionVMDescriptor<TSourceItem, TItemVM>>
      where TItemVM : SelectionItemVM<TSourceItem> {

      private Func<TSourceItem, bool> _selectableItemFilter;

      public SingleSelectionVM(
         SingleSelectionVMDescriptor<TSourceItem, TItemVM> descriptor,
         IViewModel parent,
         Func<TSourceItem, bool> selectableItemFilter
      )
         : base() {
         _selectableItemFilter = selectableItemFilter;
         Descriptor = descriptor;
         Parent = parent;
      }

      public SingleSelectionVMDescriptor<TSourceItem, TItemVM> Descriptor { get; private set; }

      public IViewModel Parent { get; set; }

      //public override bool IsValid(bool validateChildren) {
      //   // HACK
      //   // base.IsValid fails because there is a foreign VMProperty for which the dynamic field
      //   // cannot be accessed.
      //   //return ValidateProperty(Descriptor.SelectedItem).Successful;
      //   throw new NotImplementedException();
      //}

      private IEnumerable<TSourceItem> FilteredItems {
         get {
            TSourceItem currentSelection = SelectedSourceItem;
            return UnfilteredSourceItems
               .Where(item =>
                  _selectableItemFilter == null ||
                  _selectableItemFilter(item) ||
                  Object.Equals(item, currentSelection)
               )
               .ToArray();
         }
      }

      private IEnumerable<TSourceItem> UnfilteredSourceItems {
         get { /*return ViewModelExtensibility.GetForeignProerty(Parent, Descriptor.UnfilteredSourceItems);*/ throw new NotImplementedException(); }
      }

      private TSourceItem SelectedSourceItem {
         get { /*return ViewModelExtensibility.GetForeignProerty(Parent, Descriptor.SelectedSourceItem); */ throw new NotImplementedException(); }
         set { /*ViewModelExtensibility.SetForeignProperty(Parent, Descriptor.SelectedSourceItem, value);*/ throw new NotImplementedException(); }
      }

      public VMCollection<TItemVM> AllItems {
         get { return GetValue(Descriptor.AllItems); }
      }

      public TItemVM SelectedItem {
         get { return GetValue(Descriptor.SelectedItem); }
         set { SetValue(Descriptor.SelectedItem, value); }
      }

      public void Refresh() {
         throw new NotImplementedException();
         //UpdateFromSource(Descriptor.AllItems);
         //UpdateFromSource(Descriptor.SelectedItem);
         Revalidate();
      }

      public new void Revalidate() {
         //Revalidate(Descriptor.SelectedItem);
      }

      internal static SingleSelectionVMDescriptor<TSourceItem, TItemVM> CreateDescriptor(
         VMProperty<IEnumerable<TSourceItem>> allSourceItemsProperty,
         VMProperty<TSourceItem> selectedSourceItemProperty,
         VMDescriptor itemDescriptor,
         Action<
            SingleSelectionVMDescriptor<TSourceItem, TItemVM>,
            IValidationBuilder<SingleSelectionVM<TSourceItem, TItemVM>>
         > validationConfigurator
      ) {
         var builder = VMDescriptorBuilder
            .For<SingleSelectionVM<TSourceItem, TItemVM>>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();

               return new SingleSelectionVMDescriptor<TSourceItem, TItemVM> {
                  UnfilteredSourceItems = allSourceItemsProperty,
                  SelectedSourceItem = selectedSourceItemProperty,

                  AllItems = v
                     .MappedCollection(x => x.FilteredItems)
                     .Of<TItemVM>(itemDescriptor),

                  SelectedItem = v.Calculated(
                     (x) => x.FindSelectedItem(),
                     (x, value) => x.UpdateSelectedSourceItem(value)
                  ).Property()
               };
            });

         throw new NotImplementedException();
         //return validationConfigurator != null ?
         //   builder.WithValidations(validationConfigurator).Build() :
         //   builder.Build();
      }

      private TItemVM FindSelectedItem() {
         TSourceItem selected = SelectedSourceItem;
         return Object.ReferenceEquals(selected, null) ?
            null :
            AllItems.Single(vm => Object.Equals(vm.SourceItem, selected));
      }

      private void UpdateSelectedSourceItem(TItemVM vm) {
         SelectedSourceItem = vm != null ?
            vm.SourceItem :
            default(TSourceItem);
      }
   }

   public sealed class SingleSelectionVMDescriptor<TSourceItem, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {
      internal VMProperty<IEnumerable<TSourceItem>> UnfilteredSourceItems { get; set; }

      internal VMProperty<TSourceItem> SelectedSourceItem { get; set; }

      public VMCollectionProperty<TItemVM> AllItems { get; set; }

      public VMProperty<TItemVM> SelectedItem { get; set; }
   }
}
