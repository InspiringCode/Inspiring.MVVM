namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   internal sealed class DelegatePopulatorCollectionBehavior<TItemVM, TSourceObject> :
      Behavior,
      IPopulatorCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      private Func<TSourceObject, IEnumerable<TItemVM>> _customItemProvider;

      public DelegatePopulatorCollectionBehavior(
         Func<TSourceObject, IEnumerable<TItemVM>> customItemProvider
      ) {
         Contract.Requires(customItemProvider != null);
         _customItemProvider = customItemProvider;
      }

      public void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
         TSourceObject sourceObject = GetNextBehavior<IValueAccessorBehavior<TSourceObject>>()
            .GetValue(context, ValueStage.PostValidation);

         IEnumerable<TItemVM> items = _customItemProvider(sourceObject);

         try {
            collection.IsPopulating = true;
            collection.Clear();

            foreach (TItemVM item in items) {
               collection.Add(item);
            }
         } finally {
            collection.IsPopulating = false;
         }
      }
   }
}
