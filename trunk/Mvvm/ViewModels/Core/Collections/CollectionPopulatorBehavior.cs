namespace Inspiring.Mvvm.ViewModels.Core.Collections {
   using System.Collections.Generic;

   internal sealed class CollectionPopulatorBehavior<TItemVM, TItemSource> :
      Behavior,
      ICollectionPopulatorBehavior<TItemVM>
      where TItemVM : IViewModel, ICanInitializeFrom<TItemSource> {

      public void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
         var sourceAccessor = GetNextBehavior<IAccessPropertyBehavior<IEnumerable<TItemSource>>>();
         var vmFactory = GetNextBehavior<IViewModelFactoryBehavior<TItemVM>>();

         IEnumerable<TItemSource> sourceItems = sourceAccessor.GetValue(context);

         try {
            collection.IsPopulating = true;

            collection.Clear();

            foreach (TItemSource itemSource in sourceItems) {
               TItemVM vm = vmFactory.CreateInstance(context);
               vm.InitializeFrom(itemSource);
               collection.Add(vm);
            }
         } finally {
            collection.IsPopulating = false;
         }
      }
   }
}
