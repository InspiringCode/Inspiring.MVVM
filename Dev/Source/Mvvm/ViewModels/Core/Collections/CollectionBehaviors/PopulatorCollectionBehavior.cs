//namespace Inspiring.Mvvm.ViewModels.Core {
//   using System.Collections.Generic;
//   using System.Linq;

//   internal sealed class PopulatorCollectionBehavior<TItemVM, TItemSource> :
//      Behavior,
//      IPopulatorCollectionBehavior<TItemVM>
//      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

//      public void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
//         var sourceAccessor = GetNextBehavior<IValueAccessorBehavior<IEnumerable<TItemSource>>>();
//         var vmFactory = GetNextBehavior<IViewModelFactoryBehavior<TItemVM>>();

//         IEnumerable<TItemSource> sourceItems = sourceAccessor.GetValue(context);

//         var newItems = sourceItems.Select((i) => {
//            TItemVM vm = vmFactory.CreateInstance(context);
//            vm.Source = i;
//            return vm;
//         }).ToArray();

//         collection.ReplaceItems(newItems);

//         // TODO: Validation is a bit messy and distributed?
//         foreach (TItemVM item in collection) {
//            item.Kernel.Revalidate(ValidationScope.SelfOnly, ValidationMode.CommitValidValues);
//         }
//      }
//   }
//}
