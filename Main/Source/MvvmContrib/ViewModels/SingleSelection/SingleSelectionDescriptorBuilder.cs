namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   internal class SingleSelectionDescriptorBuilderBase<TDescriptor, TVM, TItemSource, TItemVM> :
      SelectionDescriptorBuilder<TDescriptor, TVM>
      where TDescriptor : SingleSelectionVMDescriptor<TItemSource, TItemVM>, new()
      where TVM : SingleSelectionBaseVM<TItemSource, TItemVM>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public SingleSelectionDescriptorBuilderBase(
         IVMDescriptor itemDescriptor,
         Func<TItemSource, bool> isActiveFilter
      ) {
         WithProperties((d, b) => {
            var v = b.GetPropertyBuilder();

            d.SelectedItem = v.VM.DelegatesTo(
               (vm) => vm.GetSelectedItem(),
               (vm, val) => vm.SetSelectedItem(val)
            );

            d.AllItems = v
               .Collection
               .PopulatedWith(vm => {
                  IEnumerable<TItemSource> selectableItems = SelectionHelpers
                     .GetSelectableSourceItems<TItemSource>(vm);

                  return selectableItems.Select(x => vm.GetItemVM(x));
               })
               .With(itemDescriptor);
         });

         WithViewModelBehaviors(b => {
            b.OverrideUpdateFromSourceProperties(
               x => x.AllSourceItems,
               x => x.SelectedSourceItem,
               x => x.AllItems,
               x => x.SelectedItem
            );
            b.OverrideUpdateSourceProperties(
               x => x.SelectedSourceItem
            );

            b.AppendBehavior(new SelectionItemViewModelCacheBehavior<TItemSource, TItemVM>(itemDescriptor));
            b.PrependBehavior(new ItemProviderBehavior<TItemSource>() { IsActiveFilter = isActiveFilter });
         });

         WithBehaviors(b => {
            b.Property(x => x.AllSourceItems).IsCached();
         });

         WithValidators(b => {
            b.EnableParentValidation(x => x.SelectedItem);
         });
      }
   }

   internal class SelectableItemSingleSelectionDescriptorBuilder<TSourceObject, TItemSource, TItemVM> :
      SingleSelectionDescriptorBuilderBase<
         SingleSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>>,
         SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>,
         TItemSource,
         SelectableItemVM<TItemSource, TItemVM>
      > where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public SelectableItemSingleSelectionDescriptorBuilder(
         Func<TItemSource, bool> isActiveFilter
       )
         : base(CreateItemDescriptor(), isActiveFilter) {

         WithDependencies(b => {
            b.OnChangeOf
               .Collection(x => x.AllItems, true)
               .Execute((vm, args) => {
                  // Initiales setzen der IsSelected property
                  if (args.ChangeType == ChangeType.CollectionPopulated) {
                     var selectedItem = vm.SelectedItem;
                     if (selectedItem != null) {
                        selectedItem.IsSelected = true;
                     }
                  }
               });

            b.OnChangeOf
               .Properties(x => x.SelectedItem)
               .Execute((vm, args) => {
                  var newItem = (SelectableItemVM<TItemSource, TItemVM>)args.NewItems.FirstOrDefault();
                  if (newItem != null && !newItem.IsSelected) {
                     newItem.IsSelected = true;
                  }

                  var oldItem = (SelectableItemVM<TItemSource, TItemVM>)args.OldItems.FirstOrDefault();
                  if (oldItem != null && oldItem.IsSelected) {
                     oldItem.IsSelected = false;
                  }
               });

            b.OnChangeOf
               .Descendant(x => x.AllItems)
               .Properties(x => x.IsSelected)
               .Execute((vm, args) => {
                  var item = (SelectableItemVM<TItemSource, TItemVM>)args.ChangedVM;

                  if (item.IsSelected) {
                     vm.SelectedItem = item;
                  } else {
                     if (vm.AllItems.All(x => !x.IsSelected)) {
                        vm.SelectedItem = null;
                     }
                  }
               });
         });
      }


      private static SelectableItemVMDescriptor<TItemVM> CreateItemDescriptor() {
         return VMDescriptorBuilder
            .OfType<SelectableItemVMDescriptor<TItemVM>>()
            .For<SelectableItemVM<TItemSource, TItemVM>>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();

               d.IsSelected = v.Property.Of<bool>();
               d.VM = v.VM.Wraps(x => x.Source).With<TItemVM>();
            })
            .WithValidators(b => b.EnableParentViewModelValidation())
            .Build();
      }
   }

   internal class CaptionSingleSelectionDescriptorBuilder<TSourceObject, TItemSource> :
      SingleSelectionDescriptorBuilderBase<
         SingleSelectionVMDescriptor<TItemSource>,
         SingleSelectionWithSourceVM<TSourceObject, TItemSource>,
         TItemSource,
         SelectionItemVM<TItemSource>
      > {

      public CaptionSingleSelectionDescriptorBuilder(
         Func<TItemSource, bool> isActiveFilter,
         IVMDescriptor itemDescriptor
      )
         : base(itemDescriptor, isActiveFilter) {
      }
   }
}
