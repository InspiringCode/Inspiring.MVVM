﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   internal sealed class PopulatorCollectionBehavior<TItemVM, TItemSource> :
      Behavior,
      IPopulatorCollectionBehavior<TItemVM>
      where TItemVM : IViewModel, ICanInitializeFrom<TItemSource> {

      public void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
         var sourceAccessor = GetNextBehavior<IValueAccessorBehavior<IEnumerable<TItemSource>>>();
         var vmFactory = GetNextBehavior<IViewModelFactoryBehavior<TItemVM>>();

         IEnumerable<TItemSource> sourceItems = sourceAccessor.GetValue(context, ValueStage.PostValidation);

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