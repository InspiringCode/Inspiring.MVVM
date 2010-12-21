namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public abstract class MultiSelectionVM<TItemSource, TItemVM> :
      ViewModel<MultiSelectionVMDescriptor<TItemSource, TItemVM>>
      where TItemVM : IViewModel {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal MultiSelectionVM(
         MultiSelectionVMDescriptor<TItemSource, TItemVM> descriptor,
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
         get { return GetValue(DescriptorBase.AllSourceItems); }
      }

      public ICollection<TItemSource> SelectedSourceItems {
         get { return GetValue(DescriptorBase.SelectedSourceItems); }
      }

      public IVMCollection<TItemVM> AllItems {
         get { return GetValue(DescriptorBase.AllItems); }
      }

      public IVMCollection<TItemVM> Selectedtems {
         get { return GetValue(DescriptorBase.SelectedItems); }
      }

      /// <summary>
      ///   Returns all source items for which the <see cref="ActiveItemFilter"/>
      ///   returns true or that are currently contained by selected items collection
      ///   of the source object.
      /// </summary>
      private IEnumerable<TItemSource> GetActiveSourceItems() {
         IEnumerable<TItemSource> allSourceItems = GetValue(DescriptorBase.AllSourceItems);
         IEnumerable<TItemSource> selectedSourceItems = GetValue(DescriptorBase.SelectedSourceItems);

         if (ActiveItemFilter == null) {
            return allSourceItems;
         }

         return allSourceItems.Where(i =>
            ActiveItemFilter(i) ||
            selectedSourceItems.Contains(i)
         );
      }
   }

   public sealed class MultiSelectionVM<TSourceObject, TItemSource, TItemVM> :
      MultiSelectionVM<TItemSource, TItemVM>,
      ICanInitializeFrom<TSourceObject>
      where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal MultiSelectionVM(
         MultiSelectionVMDescriptor<TItemSource, TItemVM> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      /// <summary>
      ///   Gets the object that holds the source items. This references the
      ///   view model that holds the <see cref="MultiSelectionVM"/> (the parent
      ///   VM is simply forwarded with this property).
      /// </summary>
      private TSourceObject SourceObject { get; set; }

      /// <inheritdoc />
      public void InitializeFrom(TSourceObject source) {
         SourceObject = source;
      }

      /// <summary>
      ///   Creates the <see cref="VMDescriptor"/> for an <see cref="MultiSelectionVM"/>
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
            .For<MultiSelectionVM<TSourceObject, TItemSource, TItemVM>>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();
               var source = c.GetPropertyBuilder(x => x.SourceObject);

               return new MultiSelectionVMDescriptor<TItemSource, TItemVM> {
                  AllSourceItems = allSourceItemsPropertyFactory(source),
                  SelectedSourceItems = selectedSourceItemsPropertyFactory(source),
                  AllItems = v.Collection.Wraps(vm => vm.GetActiveSourceItems()).With<TItemVM>(itemDescriptor),
                  SelectedItems = v.Collection.Wraps(vm => vm.SelectedSourceItems).With<TItemVM>(itemDescriptor)
               };
            })
            .WithBehaviors(c => {
               // This behavior ensures, that the 'SelectedItems' collection returns the same
               // VM instances (for the same source items) as the 'AllItems' collection.
               c.For(x => x.SelectedItems).CollectionBehaviors.Enable(
                  CollectionBehaviorKeys.Populator,
                  new LookupPopulatorCollectionBehavior<MultiSelectionVM<TSourceObject, TItemSource, TItemVM>, TItemVM, TItemSource>(
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

      /// <summary>
      ///   Returns all source items for which the <see cref="ActiveItemFilter"/>
      ///   returns true or that are currently contained by selected items collection
      ///   of the source object.
      /// </summary>
      private IEnumerable<TItemSource> GetActiveSourceItems() {
         IEnumerable<TItemSource> allSourceItems = GetValue(DescriptorBase.AllSourceItems);
         IEnumerable<TItemSource> selectedSourceItems = GetValue(DescriptorBase.SelectedSourceItems);

         if (ActiveItemFilter == null) {
            return allSourceItems;
         }

         return allSourceItems.Where(i =>
            ActiveItemFilter(i) ||
            selectedSourceItems.Contains(i)
         );
      }
   }

   public sealed class MultiSelectionVMDescriptor<TItemSource, TItemVM> :
      VMDescriptor
      where TItemVM : IViewModel {

      public VMProperty<IEnumerable<TItemSource>> AllSourceItems { get; set; }
      public VMProperty<ICollection<TItemSource>> SelectedSourceItems { get; set; }
      public VMProperty<IVMCollection<TItemVM>> AllItems { get; set; }
      public VMProperty<IVMCollection<TItemVM>> SelectedItems { get; set; }
   }
}
