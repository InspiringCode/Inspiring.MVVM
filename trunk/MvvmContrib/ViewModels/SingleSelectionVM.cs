using System.Collections.Generic;
using Inspiring.Mvvm.ViewModels.Core;
namespace Inspiring.Mvvm.ViewModels {


   public sealed class SingleSelectionVM<TSourceItem, TItemVM> :
      ViewModel<SingleSelectionVMDescriptor<TSourceItem, TItemVM>>
      where TItemVM : ViewModel, ICanInitializeFrom<TSourceItem> {

      private IAccessPropertyBehavior<IEnumerable<TSourceItem>> allItemsAccessor;
      private IAccessPropertyBehavior<TSourceItem> selectedItemAccessor;

      public static readonly SingleSelectionVMDescriptor<TSourceItem, TItemVM> Descriptor = VMDescriptorBuilder
         .For<SingleSelectionVM<TSourceItem, TItemVM>>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyFactory();

            return new SingleSelectionVMDescriptor<TSourceItem, TItemVM> {
               AllItems = vm.MappedCollection(x => x.AllItems).Of<TItemVM>(null),
            };
         })
         .Build();

      public SingleSelectionVM(
         IAccessPropertyBehavior<IEnumerable<TSourceItem>> allItemsAccessor,
         IAccessPropertyBehavior<TSourceItem> selectedItemAccessor
      )
         : base(Descriptor) {

      }

      private IEnumerable<TSourceItem> AllItems {
         get;
         set;
      }

   }

   public sealed class SingleSelectionVMDescriptor<TSource, TItemVM> : VMDescriptor {
      internal VMProperty<IEnumerable<TSource>> AllSourceItems { get; set; }

      internal VMProperty<TSource> SelectedSourceItem { get; set; }

      public VMCollectionProperty<TItemVM> AllItems { get; set; }

      public VMProperty<TItemVM> SelectedItem { get; set; }
   }
}
