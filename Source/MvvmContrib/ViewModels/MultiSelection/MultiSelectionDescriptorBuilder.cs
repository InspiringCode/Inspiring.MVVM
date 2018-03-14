namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   public class MultiSelectionDescriptorBuilderBase<TSourceObject, TDescriptor, TVM, TItemSource, TItemVM> :
      SelectionDescriptorBuilder<TDescriptor, TVM>
      where TDescriptor : MultiSelectionVMDescriptor<TItemSource, TItemVM>, new()
      where TVM : MultiSelectionBaseVM<TItemSource, TItemVM>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public MultiSelectionDescriptorBuilderBase(
         IVMDescriptor itemDescriptor,
         Func<TSourceObject, TItemSource, bool> isActiveFilter
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
                     .GetSelectableSourceItems<TSourceObject, TItemSource>(vm);

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

            b.AppendBehavior(
               new SelectionItemViewModelCacheBehavior<TItemSource, TItemVM>(itemDescriptor)
            );

            b.AppendBehavior(new ItemProviderBehavior<TSourceObject, TItemSource>() { IsActiveFilter = isActiveFilter });
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
            b.Property(x => x.SelectedItems).SupportsDisplayValueConversion();

            b.Property(x => x.SelectedItems).PrependBehavior(new SelectedItemsRefreshBehavior<TItemVM>());

            // TODO: Make this configurable.
            b.Property(x => x.SelectedSourceItems).RequiresLoadedProperty(x => x.AllSourceItems);
         });

         WithValidators(b => {
            b.EnableParentValidation(x => x.SelectedItems);
            b.EnableParentViewModelValidation();
         });
      }
   }

   public sealed class MultiSelectionDescriptorBuilder<TSourceObject, TItemSource, TItemVM> :
      MultiSelectionDescriptorBuilderBase<
         TSourceObject,
         MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>>,
         MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>,
         TItemSource,
         SelectableItemVM<TItemSource, TItemVM>
      > where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public MultiSelectionDescriptorBuilder(
         Func<TSourceObject, TItemSource, bool> isActiveFilter,
         bool isSelectedRaisesPropertyChanged
       )
         : base(CreateItemDescriptor(isSelectedRaisesPropertyChanged), isActiveFilter) {

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


      private static SelectableItemVMDescriptor<TItemVM> CreateItemDescriptor(bool isSelectedRaisesPropertyChanged) {
         return VMDescriptorBuilder
            .OfType<SelectableItemVMDescriptor<TItemVM>>()
            .For<SelectableItemVM<TItemSource, TItemVM>>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();

               d.IsSelected = v.Property.Of<bool>();
               d.VM = v.VM.Wraps(x => x.Source).With<TItemVM>();

            })
            .WithValidators(b => b.EnableParentViewModelValidation())
            .WithBehaviors(b => {
               if (!isSelectedRaisesPropertyChanged) {
                  // HACK: This behavior needs to be disabled to workaround the following problem:
                  //
                  // * A 'MultiSelection' with 'SelectableItemVM' elements (configured with 'Of<TItemVM>' instead
                  //   of 'WithCaption') is bound to a ComboBox (specifically a DevExpress WPF ComboBoxEdit).
                  // * The user changes the selected items in the control.
                  // * The ComboBox sets the selected items via 'SettableListDisplayValueBehavior'.
                  // * Each collection modification in 'SettableListDisplayValueBehavior.SynchronizeCollections' causes
                  //   the 'IsSelected' property of added or removed 'SelectableItemVM' objects to change (because of
                  //   the dependency defined in the constructor of 'MultiSelectionDescriptorBuilder').
                  // * This change raises a 'PropertyChanged' event for the 'IsSelected' property.
                  // * This 'PropertyChanged' event triggers a call of 'VMCollection.OnListChanged' with 'ListChangedType.ItemChanged'.
                  // * The 'ListChanged' event that is raised in the base 'OnListChanged' method is handled by the ComboBox.
                  // * The ComboBox reacts to the 'ListChanged' event by setting the value of the MultiSelection again.
                  // * This calls the 'SettableListDisplayValueBehavior' again. Now there are two calls of its
                  //   'SynchronizeCollections' method in the call stack, one from the initial call caused by the user
                  //   interaction and one that was triggered by the 'ListChanged' event.
                  // * That second call to 'SynchronizeCollections' might again cause 'PropertyChanged' and 'ListChanged' events.
                  // * After the code returns to the first call of 'SynchronizeCollections' and tries to modify the
                  //   next element in the collection, the code in 'SynchronizerCollectionBehavior' notices that the
                  //   collections are not in sync any more since the second call to 'SettableListDisplayValueBehavior.SynchronizeCollections'
                  //   modified the same collection that the first one was still modifying. This causes the
                  //   'SynchronizerCollectionBehavior' to throw an InvalidOperationException
                  //   (error message "The VM collection is not in sync with its source collection anymore.").
                  //
                  // To prevent this, the behavior that causes the 'PropertyChanged' event of the 'IsSelected' property
                  // to be raised is disabled here.
                  // But this is only ok if the 'MultiSelection' is used in a ComboBox. If it's used in a 'ListBox'
                  // where the control directly modifies the 'IsSelected' properties (instead of setting the whole
                  // 'SelectedItems' collection) the 'PropertyChanged' event is required to synchronize the changes
                  // to the collections.
                  b.Property(x => x.IsSelected).Disable(PropertyBehaviorKeys.ChangeNotifier);
               }
            })
            .Build();
      }
   }

   public sealed class MultiSelectionWithCaptionDescriptorBuilder<TSourceObject, TItemSource> :
      MultiSelectionDescriptorBuilderBase<
         TSourceObject,
         MultiSelectionVMDescriptor<TItemSource>,
         MultiSelectionWithSourceVM<TSourceObject, TItemSource>,
         TItemSource,
         SelectionItemVM<TItemSource>
      > {

      public MultiSelectionWithCaptionDescriptorBuilder(
         Func<TSourceObject, TItemSource, bool> isActiveFilter,
         IVMDescriptor itemDescriptor
      )
         : base(itemDescriptor, isActiveFilter) {
      }
   }
}
