namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   internal class MultiSelectionDescriptorBuilderBase<TDescriptor, TVM, TItemSource, TItemVM> :
      SelectionDescriptorBuilder<TDescriptor, TVM>
      where TDescriptor : MultiSelectionVMDescriptor<TItemSource, TItemVM>, new()
      where TVM : MultiSelectionBaseVM<TItemSource, TItemVM>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public MultiSelectionDescriptorBuilderBase(
         IVMDescriptor itemDescriptor,
         Func<TItemSource, bool> isActiveFilter
      ) {
         WithProperties((d, b) => {
            var v = b.GetPropertyBuilder();

            d.SelectedItems = v
               .Collection
               .Wraps(x => x.SelectedSourceItems)
               .With<TItemVM>(itemDescriptor);

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
               x => x.SelectedSourceItems,
               x => x.AllItems,
               x => x.SelectedItems
            );
            b.OverrideUpdateSourceProperties(
               x => x.SelectedSourceItems
            );

            b.AppendBehavior(new SelectionItemViewModelCacheBehavior<TItemSource, TItemVM>(itemDescriptor));
            b.AppendBehavior(new ItemProviderBehavior<TItemSource>() { IsActiveFilter = isActiveFilter });
         });

         WithBehaviors(b => {
            b.Property(x => x.SelectedItems).Enable(
               PropertyBehaviorKeys.ValueAccessor,
               new SelectedItemsAccessorBehavior<TVM, TItemVM, TItemSource>()
            );

            // This behavior allows a bound comobox to assign a new list to the 'SelectedItems'
            // property every time the selection changes.
            b.Property(x => x.SelectedItems).Enable(
               PropertyBehaviorKeys.DisplayValueAccessor,
               new SettableListDisplayValueBehavior<TItemVM>()
            );

            b.Property(x => x.SelectedItems).AddChangeHandler((vm, args) => {
               vm.RaisePropertyChangedForSelectedItems(); // HACK!
            });

            b.Property(x => x.AllSourceItems).IsCached();
         });

         WithValidators(b => {
            b.EnableParentValidation(x => x.SelectedItems);
            b.EnableParentViewModelValidation();
         });
      }
   }

   internal class SelectableItemMultiSelectionDescriptorBuilder<TSourceObject, TItemSource, TItemVM> :
      MultiSelectionDescriptorBuilderBase<
         MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>>,
         MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>,
         TItemSource,
         SelectableItemVM<TItemSource, TItemVM>
      > where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public SelectableItemMultiSelectionDescriptorBuilder(
         Func<TItemSource, bool> isActiveFilter
       )
         : base(CreateItemDescriptor(), isActiveFilter) {

         WithDependencies(b => {
            b.OnChangeOf
               .Collection(x => x.SelectedItems, true)
               .Execute((vm, args) => {
                  if (args.ChangeType == ChangeType.CollectionPopulated || args.ChangeType == ChangeType.AddedToCollection) {
                     foreach (SelectableItemVM<TItemSource, TItemVM> item in args.NewItems) {
                        item.IsSelected = true;
                     }
                  }

                  if (args.ChangeType == ChangeType.RemovedFromCollection) {
                     foreach (SelectableItemVM<TItemSource, TItemVM> item in args.OldItems) {
                        item.IsSelected = false;
                     }
                  }
               });

            b.OnChangeOf
               .Descendant(x => x.AllItems)
               .Properties(x => x.IsSelected)
               .Execute(HandleIsSelectedChanged);

            // We have to handle the change on 'SelectedItems' too because 'AllItems' may
            // not be loaded.
            b.OnChangeOf
               .Descendant(x => x.SelectedItems)
               .Properties(x => x.IsSelected)
               .Execute(HandleIsSelectedChanged);

            b.OnChangeOf
               .Collection(x => x.AllItems, true)
               .Execute((vm, args) => {
                  if (args.ChangeType == ChangeType.CollectionPopulated) {
                     vm.LoadSelectedItems();
                  }
               });
         });
      }

      private static void HandleIsSelectedChanged(
         MultiSelectionWithSourceVM<TSourceObject,
         TItemSource, TItemVM> vm, ChangeArgs args
      ) {
         var item = (SelectableItemVM<TItemSource, TItemVM>)args.ChangedVM;

         if (item.IsSelected) {
            if (!vm.SelectedItems.Contains(item)) {
               vm.SelectedItems.Add(args.ChangedVM);
            }
         } else {
            vm.SelectedItems.Remove(args.ChangedVM);
         }
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

   internal class CaptionMultiSelectionDescriptorBuilder<TSourceObject, TItemSource> :
      MultiSelectionDescriptorBuilderBase<
         MultiSelectionVMDescriptor<TItemSource>,
         MultiSelectionWithSourceVM<TSourceObject, TItemSource>,
         TItemSource,
         SelectionItemVM<TItemSource>
      > {

      public CaptionMultiSelectionDescriptorBuilder(
         Func<TItemSource, bool> isActiveFilter,
         IVMDescriptor itemDescriptor
      )
         : base(itemDescriptor, isActiveFilter) {
      }
   }
}
