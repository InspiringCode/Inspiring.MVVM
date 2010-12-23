namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public sealed class MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM> :
      MultiSelectionVM<TItemSource, TItemVM>,
      ICanInitializeFrom<TSourceObject>
      where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal MultiSelectionWithSourceVM(
         MultiSelectionVMDescriptor<TItemSource, TItemVM> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
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
      internal static MultiSelectionVMDescriptor<TItemSource, TItemVM> CreateDescriptor(
         VMDescriptorBase itemDescriptor,
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<ICollection<TItemSource>>> selectedSourceItemsPropertyFactory,
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<IEnumerable<TItemSource>>> allSourceItemsPropertyFactory
      ) {
         return VMDescriptorBuilder
            .OfType<MultiSelectionVMDescriptor<TItemSource, TItemVM>>()
            .For<MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var source = c.GetPropertyBuilder(x => x.SourceObject);

               d.AllSourceItems = allSourceItemsPropertyFactory(source);
               d.SelectedSourceItems = selectedSourceItemsPropertyFactory(source);
               d.AllItems = v.Collection.Wraps(vm => vm.GetActiveSourceItems()).With<TItemVM>(itemDescriptor);
               d.SelectedItems = v.Collection.Wraps(vm => vm.SelectedSourceItems).With<TItemVM>(itemDescriptor);
            })
            .WithBehaviors(c => {
               // This behavior ensures, that the 'SelectedItems' collection returns the same
               // VM instances (for the same source items) as the 'AllItems' collection.
               c.For(x => x.SelectedItems).CollectionBehaviors.Enable(
                  CollectionBehaviorKeys.Populator,
                  new LookupPopulatorCollectionBehavior<MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>, TItemVM, TItemSource>(
                     multiSelectionVM => multiSelectionVM.AllItems
                  )
               );

               // This behavior allows a bound comobox to assign a new list to the 'SelectedItems'
               // property every time the selection changes.
               c.For(x => x.SelectedItems).Enable(
                  BehaviorKeys.DisplayValueAccessor,
                  new SettableListDisplayValueBehavior<TItemVM>()
               );
            })
            .Build();
      }
   }

   public sealed class MultiSelectionWithSourceVM<TSourceObject, TItemSource> :
      MultiSelectionVM<TItemSource> {

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
      internal TSourceObject SourceObject { get; set; }

      /// <inheritdoc />
      public void InitializeFrom(TSourceObject source) {
         SourceObject = source;
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
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<ICollection<TItemSource>>> selectedSourceItemsPropertyFactory,
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<IEnumerable<TItemSource>>> allSourceItemsPropertyFactory
      ) {
         return VMDescriptorBuilder
            .OfType<MultiSelectionVMDescriptor<TItemSource>>()
            .For<MultiSelectionWithSourceVM<TSourceObject, TItemSource>>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var source = c.GetPropertyBuilder(x => x.SourceObject);

               d.AllSourceItems = allSourceItemsPropertyFactory(source);
               d.SelectedSourceItems = selectedSourceItemsPropertyFactory(source);
               d.AllItems = v.Collection.Wraps(vm => vm.GetActiveSourceItems()).With<SelectionItemVM<TItemSource>>(itemDescriptor);
               d.SelectedItems = v.Collection.Wraps(vm => vm.SelectedSourceItems).With<SelectionItemVM<TItemSource>>(itemDescriptor);
            })
            .WithBehaviors(c => {
               // This behavior ensures, that the 'SelectedItems' collection returns the same
               // VM instances (for the same source items) as the 'AllItems' collection.
               c.For(x => x.SelectedItems).CollectionBehaviors.Enable(
                  CollectionBehaviorKeys.Populator,
                  new LookupPopulatorCollectionBehavior<MultiSelectionWithSourceVM<TSourceObject, TItemSource>, SelectionItemVM<TItemSource>, TItemSource>(
                     multiSelectionVM => multiSelectionVM.AllItems
                  )
               );

               // This behavior allows a bound comobox to assign a new list to the 'SelectedItems'
               // property every time the selection changes.
               c.For(x => x.SelectedItems).Enable(
                  BehaviorKeys.DisplayValueAccessor,
                  new SettableListDisplayValueBehavior<SelectionItemVM<TItemSource>>()
               );
            })
            .Build();
      }
   }
}
