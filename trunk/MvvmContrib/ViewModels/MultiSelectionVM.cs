namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public sealed class MultiSelectionVM<TSourceObject, TItemSource, TItemVM> :
      ViewModel<MultiSelectionVMDescriptor<TItemSource, TItemVM>>,
      ICanInitializeFrom<TSourceObject>
      where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

      public MultiSelectionVM(
        IServiceLocator serviceLocator,
        VMDescriptorBase itemDescriptor,
        Func<IVMPropertyBuilder<TSourceObject>, VMProperty<ICollection<TItemSource>>> selectedSourceItemsPropertyFactory,
        Func<IVMPropertyBuilder<TSourceObject>, VMProperty<IEnumerable<TItemSource>>> allSourceItemsPropertyFactory
     )
         : base(
            CreateDescriptor(itemDescriptor, selectedSourceItemsPropertyFactory, allSourceItemsPropertyFactory),
            serviceLocator
         ) {
      }

      public TSourceObject SourceObject { get; private set; }

      public void InitializeFrom(TSourceObject source) {
         SourceObject = source;
      }

      public Func<TItemSource, bool> ActiveItemFilter {
         get;
         set;
      }

      private IVMCollection<TItemVM> AllItems {
         get { return GetValue(DescriptorBase.AllItems); }
      }

      private ICollection<TItemSource> SelectedSourceItems {
         get { return GetValue(DescriptorBase.SelectedSourceItems); }
      }

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

      private static MultiSelectionVMDescriptor<TItemSource, TItemVM> CreateDescriptor(
         VMDescriptorBase itemDescriptor,
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<ICollection<TItemSource>>> selectedSourceItemsPropertyFactory,
         Func<IVMPropertyBuilder<TSourceObject>, VMProperty<IEnumerable<TItemSource>>> allSourceItemsPropertyFactory
      ) {
         return VMDescriptorBuilder
            .For<MultiSelectionVM<TSourceObject, TItemSource, TItemVM>>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();
               var fac = c.GetPropertyBuilder(x => x.SourceObject);

               return new MultiSelectionVMDescriptor<TItemSource, TItemVM> {
                  AllSourceItems = allSourceItemsPropertyFactory(fac),
                  SelectedSourceItems = selectedSourceItemsPropertyFactory(fac),
                  AllItems = v.Collection.Wraps(x => x.GetActiveSourceItems()).With<TItemVM>(itemDescriptor),
                  SelectedItems = v.Collection.Wraps(x => x.SelectedSourceItems).With<TItemVM>(itemDescriptor)
               };
            })
            .WithBehaviors(c => {
               c.For(x => x.SelectedItems).CollectionBehaviors.Enable(
                  CollectionBehaviorKeys.Populator,
                  new LookupPopulatorCollectionBehavior<MultiSelectionVM<TSourceObject, TItemSource, TItemVM>, TItemVM, TItemSource>(
                     multiSelectionVM => multiSelectionVM.AllItems
                  )
               );
            })
            .Build();
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
