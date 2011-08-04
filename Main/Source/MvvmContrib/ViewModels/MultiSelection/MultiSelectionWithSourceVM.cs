namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   public sealed class MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM> :
      MultiSelectionVM<TItemSource, TItemVM>,
      IHasReadonlySourceObject<TSourceObject>,
      IHasSourceObject<TSourceObject>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal MultiSelectionWithSourceVM(
         MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> descriptor,
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
      ///   view model that holds the <see cref="MultiSelectionWithSourceVM"/> (the parent
      ///   VM is simply forwarded with this property).
      /// </summary>
      private TSourceObject SourceObject { get; set; }

      /// <inheritdoc />
      public void InitializeFrom(TSourceObject source) {
         SourceObject = source;
         //Kernel.Revalidate(Descriptor.SelectedItems, ValidationMode.DiscardInvalidValues); // TODO: Unify validation on first access handling
      }

      /// <summary>
      ///   Creates the <see cref="VMDescriptor"/> for an <see cref="MultiSelectionWithSourceVM"/>
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
      internal static MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> CreateDescriptor(
         Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<ICollection<TItemSource>>> selectedSourceItemsPropertyFactory,
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
            .OfType<MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>>>()
            .For<MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var source = c.GetPropertyBuilder(x => x.SourceObject);

               d.AllSourceItems = allSourceItemsPropertyFactory(source);
               d.SelectedSourceItems = selectedSourceItemsPropertyFactory(source);

               d.AllItems = v.Collection
                  .Wraps(vm => vm.GetActiveSourceItems())
                  .With<SelectableItemVM<TItemSource, TItemVM>>(itemDescriptor);

               d.SelectedItems = v.Collection
                  .Wraps(vm => vm.SelectedSourceItems)
                  .With<SelectableItemVM<TItemSource, TItemVM>>(itemDescriptor);
            })
            .WithBehaviors(c => {
               // This behavior ensures, that the 'SelectedItems' collection returns the same
               // VM instances (for the same source items) as the 'AllItems' collection.
               c.Property(x => x.SelectedItems).Enable(
                  PropertyBehaviorKeys.ValueAccessor,
                  new LookupPopulatorCollectionBehavior<MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>, SelectableItemVM<TItemSource, TItemVM>, TItemSource>(
                     multiSelectionVM => multiSelectionVM.AllItems
                  )
               );

               // This behavior allows a bound comobox to assign a new list to the 'SelectedItems'
               // property every time the selection changes.
               c.Property(x => x.SelectedItems).Enable(
                  PropertyBehaviorKeys.DisplayValueAccessor,
                  new SettableListDisplayValueBehavior<SelectableItemVM<TItemSource, TItemVM>>()
               );

               c.Property(x => x.SelectedItems).AddChangeHandler((vm, args) => {
                  vm.OnPropertyChanged("SelectedItems"); // HACK!
               });
            })
            // TODO: Make this configurable?
            .WithValidators(b => {
               b.EnableParentViewModelValidation();
               b.CheckCollection(x => x.SelectedItems).Custom(args => {
                  var invalidItems = args
                     .Items
                     .Where(i => args
                        .Owner
                        .NonExistingSelectedSourceItems
                        .Contains(i.Source));


                  foreach (var item in invalidItems) {
                     // TODO: Let the user specify the message.
                     args.AddError(item, "Das gewählte Element ist nicht vorhanden.");
                  }
               });
            })
            .WithDependencies(b => {
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
                  .Execute((vm, args) => {
                     if ((bool)args.ChangedVM.Kernel.GetValue(args.ChangedProperty)) {
                        if (!vm.SelectedItems.Contains(args.ChangedVM)) {
                           vm.SelectedItems.Add(args.ChangedVM);
                        }
                     } else {
                        vm.SelectedItems.Remove(args.ChangedVM);
                     }
                  });
               b.OnChangeOf
                  .Collection(x => x.AllItems, true)
                  .Execute((vm, args) => {
                     if (args.ChangeType == ChangeType.CollectionPopulated) {
                        vm.Load(vm.Descriptor.SelectedItems);
                     }
                  });
            });

         //if (enableValidation) {
         //   builder = builder.WithValidators(b => {
         //      b.EnableParentValidation(x => x.SelectedItems);
         //   });
         //}

         return builder.WithViewModelBehaviors(b => {
            b.OverrideUpdateFromSourceProperties(
               x => x.AllSourceItems,
               x => x.SelectedSourceItems,
               x => x.AllItems,
               x => x.SelectedItems
            );
            b.OverrideUpdateSourceProperties(
               x => x.SelectedSourceItems
            );
            if (enableUndo) {
               b.EnableUndo();
            }
         })
         .Build();
      }
   }

   public sealed class MultiSelectionWithSourceVM<TSourceObject, TItemSource> :
      MultiSelectionVM<TItemSource>,
      IHasSourceObject<TSourceObject> {

      public MultiSelectionWithSourceVM(
         MultiSelectionVMDescriptor<TItemSource> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      /// <summary>
      ///   Gets the object that holds the source items. This references the
      ///   view model that holds the <see cref="MultiSelectionWithSourceVM"/> (the parent
      ///   VM is simply forwarded with this property).
      /// </summary>
      public TSourceObject Source { get; set; }

      /// <inheritdoc />
      public void InitializeFrom(TSourceObject source) {
         Source = source;
         //Kernel.Revalidate(Descriptor.SelectedItems, ValidationMode.DiscardInvalidValues); // TODO: Unify validation on first access handling
      }

      /// <summary>
      ///   Creates the <see cref="VMDescriptor"/> for an <see cref="MultiSelectionWithSourceVM"/>
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
      internal static MultiSelectionVMDescriptor<TItemSource> CreateDescriptor(
         SelectionItemVMDescriptor itemDescriptor,
         Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<ICollection<TItemSource>>> selectedSourceItemsPropertyFactory,
         Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<IEnumerable<TItemSource>>> allSourceItemsPropertyFactory,
         bool enableValidation,
         bool enableUndo
      ) {
         var builder = VMDescriptorBuilder
            .OfType<MultiSelectionVMDescriptor<TItemSource>>()
            .For<MultiSelectionWithSourceVM<TSourceObject, TItemSource>>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var source = c.GetPropertyBuilder(x => x.Source);

               d.AllSourceItems = allSourceItemsPropertyFactory(source);
               d.SelectedSourceItems = selectedSourceItemsPropertyFactory(source);
               d.AllItems = v.Collection.Wraps(vm => vm.GetActiveSourceItems()).With<SelectionItemVM<TItemSource>>(itemDescriptor);
               d.SelectedItems = v.Collection.Wraps(vm => vm.SelectedSourceItems).With<SelectionItemVM<TItemSource>>(itemDescriptor);
            })
            .WithBehaviors(c => {
               // This behavior ensures, that the 'SelectedItems' collection returns the same
               // VM instances (for the same source items) as the 'AllItems' collection.
               c.Property(x => x.SelectedItems).Enable(
                  PropertyBehaviorKeys.ValueAccessor,
                  new LookupPopulatorCollectionBehavior<MultiSelectionWithSourceVM<TSourceObject, TItemSource>, SelectionItemVM<TItemSource>, TItemSource>(
                     multiSelectionVM => multiSelectionVM.AllItems
                  )
               );

               // This behavior allows a bound comobox to assign a new list to the 'SelectedItems'
               // property every time the selection changes.
               c.Property(x => x.SelectedItems).Enable(
                  PropertyBehaviorKeys.DisplayValueAccessor,
                  new SettableListDisplayValueBehavior<SelectionItemVM<TItemSource>>()
               );

               c.Property(x => x.SelectedItems).AddChangeHandler((vm, args) => {
                  vm.OnPropertyChanged("SelectedItems"); // HACK!
               });
            })
            // TODO: Make this configurable?
            .WithValidators(b => {
               b.EnableParentViewModelValidation();
               b.CheckCollection(x => x.SelectedItems).Custom(args => {
                  var invalidItems = args
                     .Items
                     .Where(i => args
                        .Owner
                        .NonExistingSelectedSourceItems
                        .Contains(i.Source));


                  foreach (var item in invalidItems) {
                     // TODO: Let the user specify the message.
                     args.AddError(item, "Das gewählte Element ist nicht vorhanden.");
                  }
               });
            });

         //if (enableValidation) {
         //   builder = builder.WithValidators(b => {
         //      b.EnableParentValidation(x => x.SelectedItems);
         //   });
         //}

         return builder.WithViewModelBehaviors(b => {
            b.OverrideUpdateFromSourceProperties(
               x => x.AllSourceItems,
               x => x.SelectedSourceItems,
               x => x.AllItems,
               x => x.SelectedItems
            );
            b.OverrideUpdateSourceProperties(
               x => x.SelectedSourceItems // TODO: Is this enough? Rethink disconnected SelectionVMs...
            );

            if (enableUndo) {
               b.EnableUndo();
            }
         })
         .Build();
      }
   }
}
