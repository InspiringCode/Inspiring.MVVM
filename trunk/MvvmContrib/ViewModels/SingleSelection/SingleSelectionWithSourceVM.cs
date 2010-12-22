namespace Inspiring.Mvvm.ViewModels.SingleSelection {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public sealed class SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM> :
      SingleSelectionVM<TItemSource, TItemVM>,
      ICanInitializeFrom<TSourceObject>
      where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal SingleSelectionWithSourceVM(
         SingleSelectionVMDescriptor<TItemSource, TItemVM> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
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
      internal static SingleSelectionVMDescriptor<TItemSource, TItemVM> CreateDescriptor(
         VMDescriptorBase itemDescriptor,
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<TItemSource>> selectedSourceItemPropertyFactory,
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<IEnumerable<TItemSource>>> allSourceItemsPropertyFactory
      ) {
         return VMDescriptorBuilder
            .For<SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();
               var source = c.GetPropertyBuilder(x => x.SourceObject);

               return new SingleSelectionVMDescriptor<TItemSource, TItemVM> {
                  AllSourceItems = allSourceItemsPropertyFactory(source),
                  SelectedSourceItem = selectedSourceItemPropertyFactory(source),
                  AllItems = v.Collection.Wraps(vm => vm.GetActiveSourceItems()).With<TItemVM>(itemDescriptor),
                  SelectedItem = v.VM.DelegatesTo(
                     vm => vm.SelectedSourceItem != null ?
                        vm.AllItems.Single(i => Object.Equals(i.Source, vm.SelectedSourceItem)) :
                        default(TItemVM),
                     (vm, value) => vm.SelectedSourceItem = value.Source
                  )
               };
            })
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
            .Build();
      }
   }

   public sealed class SingleSelectionWithSourceVM<TSourceObject, TItemSource> :
      SingleSelectionVM<TItemSource> {

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
      private TSourceObject SourceObject { get; set; }

      /// <inheritdoc />
      public void InitializeFrom(TSourceObject source) {
         SourceObject = source;
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
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<TItemSource>> selectedSourceItemsPropertyFactory,
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<IEnumerable<TItemSource>>> allSourceItemsPropertyFactory
      ) {
         return VMDescriptorBuilder
            .For<SingleSelectionWithSourceVM<TSourceObject, TItemSource>>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();
               var source = c.GetPropertyBuilder(x => x.SourceObject);

               return new SingleSelectionVMDescriptor<TItemSource> {
                  AllSourceItems = allSourceItemsPropertyFactory(source),
                  SelectedSourceItem = selectedSourceItemsPropertyFactory(source),
                  AllItems = v.Collection.Wraps(vm => vm.GetActiveSourceItems()).With<SelectionItemVM<TItemSource>>(itemDescriptor),
                  SelectedItem = v.VM.DelegatesTo(
                      vm => vm.AllItems.Single(i => Object.Equals(i.Source, vm.SelectedSourceItem)),
                      (vm, value) => vm.SelectedSourceItem = value.Source
                   )
               };
            })
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
            .Build();
      }
   }
}
