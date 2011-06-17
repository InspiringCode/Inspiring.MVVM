namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   public sealed class SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM> :
      SingleSelectionVM<TItemSource, TItemVM>,
      IHasReadonlySourceObject<TSourceObject>,
      IHasSourceObject<TSourceObject>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal SingleSelectionWithSourceVM(
         SingleSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      TSourceObject IHasReadonlySourceObject<TSourceObject>.Source {
         get { return SourceObject; }
      }

      TSourceObject IHasSourceObject<TSourceObject>.Source {
         get { return SourceObject; }
         set { SourceObject = value; }
      }

      /// <summary>
      ///   Gets the object that holds the source items. This references the
      ///   view model that holds the <see cref="SingleSelectionWithSourceVM"/> (the parent
      ///   VM is simply forwarded with this property).
      /// </summary>
      private TSourceObject SourceObject { get; set; }

      /// <inheritdoc />
      public void InitializeFrom(TSourceObject source) {
         SourceObject = source;
         //Kernel.Revalidate(Descriptor.SelectedItem, ValidationMode.DiscardInvalidValues); // TODO: Unify validation on first access handling
      }

      /// <summary>
      ///   Creates the <see cref="VMDescriptor"/> for an <see cref="SingleSelectionWithSourceVM"/>
      ///   instance.
      /// </summary>
      /// <param name="itemDescriptor">
      ///   The <see cref="VMDescriptor"/> that is use for the items of the 
      ///   <see cref="AllItems"/> and <see cref="SelectedItems"/> collections.
      /// </param>
      /// <param name="selectedSourceItemsPropertyFactory">
      ///   A function that should create a VM property for that maps or delegates
      ///   to the selected items of the source object.
      /// </param>
      /// <param name="allSourceItemsPropertyFactory">
      ///   A function that should create a VM property that returns all source
      ///   items. This may be a delegated property that returns a constant list.
      /// </param>
      internal static SingleSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> CreateDescriptor(
         Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<TItemSource>> selectedSourceItemPropertyFactory,
         Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<IEnumerable<TItemSource>>> allSourceItemsPropertyFactory,
         bool enableValidation,
         bool enableUndo
      ) {

         SelectableItemVMDescriptor<TItemVM> itemDescriptor = VMDescriptorBuilder
            .OfType<SelectableItemVMDescriptor<TItemVM>>()
            .For<SelectableItemVM<TItemSource, TItemVM>>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();

               d.IsSelected = v.Property.Of<bool>();
               d.VM = v.VM.Wraps(x => x.Source).With<TItemVM>();

            })
            .WithValidators(b => b.EnableParentViewModelValidation())
            .Build();

         var builder = VMDescriptorBuilder
            .OfType<SingleSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>>>()
            .For<SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var source = c.GetPropertyBuilder(x => x.SourceObject);

               d.AllSourceItems = allSourceItemsPropertyFactory(source);
               d.SelectedSourceItem = selectedSourceItemPropertyFactory(source);
               d.AllItems = v.Collection
                  .Wraps(vm => vm.GetActiveSourceItems())
                  .With<SelectableItemVM<TItemSource, TItemVM>>(itemDescriptor);

               d.SelectedItem = v.VM.DelegatesTo(
                  vm => vm.SelectedSourceItem != null ?
                     vm.AllItems.Single(i => Object.Equals(i.VM.Source, vm.SelectedSourceItem)) :
                     default(SelectableItemVM<TItemSource, TItemVM>),
                     (vm, value) => vm.SetValue(vm.Descriptor.SelectedSourceItem, value != null ? value.Source : default(TItemSource))
               );
            })
            .WithBehaviors(b => {
               b.Property(x => x.SelectedItem).RequiresLoadedProperty(x => x.AllItems);
            })
            .WithViewModelBehaviors(b => {
               b.OverrideUpdateFromSourceProperties(
                  x => x.AllSourceItems,
                  x => x.SelectedSourceItem,
                  x => x.AllItems,
                  x => x.SelectedItem
               );
               b.OverrideUpdateSourceProperties(
                  x => x.SelectedSourceItem
               );
            })
            // TODO: Make this configurable?
            .WithValidators(b => {
               b.Check(x => x.SelectedItem).Custom(args => {
                  if (args.Value != null &&
                      args.Owner.NonExistingSelectedSourceItem.HasValue &&
                      Object.Equals(args.Value.Source, args.Owner.NonExistingSelectedSourceItem.Value)
                  ) {
                     // TODO: Let the user specify the message.
                     args.AddError("Das gewählte Element ist nicht vorhanden.");
                  }
               });
            })
            .WithDependencies(b => {
               // Initiales setzen der IsSelected property
               b.OnChangeOf
                  .Collection(x => x.AllItems, true)
                  .Execute((vm, args) => {
                     if (args.ChangeType != ChangeType.CollectionPopulated) {
                        return;
                     }
                     var selectedItem = vm.AllItems.SingleOrDefault(i => Object.Equals(i.VM.Source, vm.SelectedSourceItem));
                     if (selectedItem != null) {
                        selectedItem.IsSelected = true;
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
                     if ((bool)args.ChangedVM.Kernel.GetValue(args.ChangedProperty)) {
                        vm.SelectedItem = (SelectableItemVM<TItemSource, TItemVM>)args.ChangedVM;
                     } else {
                        if (!vm.AllItems.Any(x => x.IsSelected)) {
                           vm.SelectedItem = null;
                        }
                     }
                  });

               b.OnChangeOf
                  .Collection(x => x.AllItems, true)
                  .Execute((vm, args) => {
                     if (args.ChangeType == ChangeType.CollectionPopulated) {
                        vm.Load(vm.Descriptor.SelectedItem);
                     }
                  });
            });

         //if (enableValidation) {
         //   builder = builder.WithValidators(b => {
         //      b.EnableParentValidation(x => x.SelectedItem);
         //   });
         //}

         if (enableUndo) {
            builder = builder.WithViewModelBehaviors(b => {
               b.EnableUndo();
            });
         }
         //.WithBehaviors(c => {
         //   // This behavior ensures, that the 'SelectedItems' collection returns the same
         //   // VM instances (for the same source items) as the 'AllItems' collection.
         //   c.For(x => x.SelectedItem).CollectionBehaviors.Enable(
         //      CollectionBehaviorKeys.Populator,
         //      new LookupPopulatorCollectionBehavior<SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>, TItemVM, TItemSource>(
         //         SingleSelectionVM => SingleSelectionVM.AllItems
         //      )
         //   );

         //   // This behavior allows a bound comobox to assign a new list to the 'SelectedItems'
         //   // property every time the selection changes.
         //   c.For(x => x.SelectedItem).Enable(
         //      BehaviorKeys.DisplayValueAccessor,
         //      new SettableListDisplayValueBehavior<TItemVM>()
         //   );
         //})

         return builder.Build();
      }
   }

   public sealed class SingleSelectionWithSourceVM<TSourceObject, TItemSource> :
      SingleSelectionVM<TItemSource>,
      IHasSourceObject<TSourceObject> {

      public SingleSelectionWithSourceVM(
         SingleSelectionVMDescriptor<TItemSource> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      /// <summary>
      ///   Gets the object that holds the source items. This references the
      ///   view model that holds the <see cref="SingleSelectionWithSourceVM"/> (the parent
      ///   VM is simply forwarded with this property).
      /// </summary>
      public TSourceObject Source { get; set; }

      /// <inheritdoc />
      public void InitializeFrom(TSourceObject source) {
         Source = source;
         //Kernel.Revalidate(Descriptor.SelectedItem, ValidationMode.DiscardInvalidValues); // TODO: Unify validation on first access handling
      }

      /// <summary>
      ///   Creates the <see cref="VMDescriptor"/> for an <see cref="SingleSelectionWithSourceVM"/>
      ///   instance.
      /// </summary>
      /// <param name="itemDescriptor">
      ///   The <see cref="VMDescriptor"/> that is use for the items of the 
      ///   <see cref="AllItems"/> and <see cref="SelectedItems"/> collections.
      /// </param>
      /// <param name="selectedSourceItemsPropertyFactory">
      ///   A function that should create a VM property for that maps or delegates
      ///   to the selected items of the source object.
      /// </param>
      /// <param name="allSourceItemsPropertyFactory">
      ///   A function that should create a VM property that returns all source
      ///   items. This may be a delegated property that returns a constant list.
      /// </param>
      internal static SingleSelectionVMDescriptor<TItemSource> CreateDescriptor(
         SelectionItemVMDescriptor itemDescriptor,
         Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<TItemSource>> selectedSourceItemsPropertyFactory,
         Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<IEnumerable<TItemSource>>> allSourceItemsPropertyFactory,
         bool enableValidation,
         bool enableUndo
      ) {
         var builder = VMDescriptorBuilder
            .OfType<SingleSelectionVMDescriptor<TItemSource>>()
            .For<SingleSelectionWithSourceVM<TSourceObject, TItemSource>>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var source = c.GetPropertyBuilder(x => x.Source);

               d.AllSourceItems = allSourceItemsPropertyFactory(source);
               d.SelectedSourceItem = selectedSourceItemsPropertyFactory(source);
               d.AllItems = v.Collection.Wraps(vm => vm.GetActiveSourceItems()).With<SelectionItemVM<TItemSource>>(itemDescriptor);
               d.SelectedItem = v.VM.DelegatesTo(
                 vm => vm.SelectedSourceItem != null ?
                    vm.AllItems.Single(i => Object.Equals(i.Source, vm.SelectedSourceItem)) :
                    default(SelectionItemVM<TItemSource>),
                    (vm, value) => vm.SetValue(vm.Descriptor.SelectedSourceItem, value != null ? value.Source : default(TItemSource))
              );
            })
            .WithBehaviors(b => {
               b.Property(x => x.SelectedItem).RequiresLoadedProperty(x => x.AllItems);
            })
            .WithViewModelBehaviors(b => {
               b.OverrideUpdateFromSourceProperties(
                  x => x.AllSourceItems,
                  x => x.SelectedSourceItem,
                  x => x.AllItems,
                  x => x.SelectedItem
               );
               b.OverrideUpdateSourceProperties(
                  x => x.SelectedSourceItem
               );
            })
            // TODO: Make this configurable?
            .WithValidators(b => {
               b.Check(x => x.SelectedItem).Custom(args => {
                  if (args.Value != null &&
                      args.Owner.NonExistingSelectedSourceItem.HasValue &&
                      Object.Equals(args.Value.Source, args.Owner.NonExistingSelectedSourceItem.Value)
                  ) {
                     // TODO: Let the user specify the message.
                     args.AddError("Das gewählte Element ist nicht vorhanden.");
                  }
               });
            });

         //if (enableValidation) {
         //   builder = builder.WithValidators(b => {
         //      b.EnableParentValidation(x => x.SelectedItem);
         //   });
         //}

         if (enableUndo) {
            builder = builder.WithViewModelBehaviors(b => {
               b.EnableUndo();
            });
         }


         //.WithBehaviors(c => {
         //   // This behavior ensures, that the 'SelectedItems' collection returns the same
         //   // VM instances (for the same source items) as the 'AllItems' collection.
         //   c.For(x => x.SelectedItem).CollectionBehaviors.Enable(
         //      CollectionBehaviorKeys.Populator,
         //      new LookupPopulatorCollectionBehavior<SingleSelectionWithSourceVM<TSourceObject, TItemSource>, SelectionItemVM<TItemSource>, TItemSource>(
         //         SingleSelectionVM => SingleSelectionVM.AllItems
         //      )
         //   );

         //   // This behavior allows a bound comobox to assign a new list to the 'SelectedItems'
         //   // property every time the selection changes.
         //   c.For(x => x.SelectedItem).Enable(
         //      BehaviorKeys.DisplayValueAccessor,
         //      new SettableListDisplayValueBehavior<SelectionItemVM<TItemSource>>()
         //   );
         //})
         return builder.Build();
      }
   }
}
