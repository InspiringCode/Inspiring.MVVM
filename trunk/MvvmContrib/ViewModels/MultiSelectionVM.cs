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
      where TItemVM : IViewModel, ICanInitializeFrom<TItemSource> {

      public static readonly MultiSelectionVMDescriptor<TItemSource, TItemVM> Descriptor = VMDescriptorBuilder
         .For<MultiSelectionVM<TSourceObject, TItemSource, TItemVM>>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyFactory();

            return new MultiSelectionVMDescriptor<TItemSource, TItemVM> {

            };
         })
         .Build();

      public MultiSelectionVM(
         IServiceLocator serviceLocator)
         : base(Descriptor, serviceLocator) {
      }

      public TSourceObject SourceObject { get; private set; }

      public void InitializeFrom(TSourceObject source) {
         SourceObject = source;
      }

      public Func<TItemSource, bool> ActiveItemFilter {
         get;
         set;
      }

      private IEnumerable<TItemSource> GetActiveSourceItems() {
         IEnumerable<TItemSource> allSourceItems = GetValue(Descriptor.AllSourceItems);
         IEnumerable<TItemSource> selectedSourceItems = GetValue(Descriptor.SelectedSourceItems);

         if (ActiveItemFilter == null) {
            return allSourceItems;
         }

         return allSourceItems.Where(i =>
            ActiveItemFilter(i) ||
            selectedSourceItems.Contains(i)
         );
      }

      //private MultiSelectionVMDescriptor<TItemSource, TItemVM> CreateDescriptor(
      //   Func<IVMPropertyFactory<TSourceObject, TSourceObject>, VMProperty<ICollection<TItemSource>>> selectedSourceItemsPropertyFactory,
      //   Func<IVMPropertyFactory<TSourceObject, TSourceObject>, VMProperty<ICollection<TItemSource>>> allSourceItemsPropertyFactory
      //) {
      //   var x = VMDescriptorBuilder
      //      .For<MultiSelectionVM<TSourceObject, TItemSource, TItemVM>>()
      //      .CreateDescriptor(c => {
      //         var v = c.GetPropertyFactory(x => x.SourceObject);
      //         return new MultiSelectionVMDescriptor<TItemSource, TItemVM> {
      //            AllSourceItems = allSourceItemsPropertyFactory(v)
      //            //SelectedSourceItems = selectedSourceItemsPropertyFactory(v)
      //         };
      //      })
      //      .Build();
      //}
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
