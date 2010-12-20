namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class LookupPopulatorCollectionBehavior<TVM, TItemVM, TItemSource> :
      Behavior,
      IPopulatorCollectionBehavior<TItemVM>
      where TVM : IViewModel
      where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

      private Func<TVM, IEnumerable<TItemVM>> _lookupSourceProvider;

      public LookupPopulatorCollectionBehavior(Func<TVM, IEnumerable<TItemVM>> lookupSourceProvider) {
         Contract.Requires(lookupSourceProvider != null);

         _lookupSourceProvider = lookupSourceProvider;
      }

      public void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
         IEnumerable<TItemVM> lookupSource = _lookupSourceProvider((TVM)context.VM);

         Dictionary<TItemSource, TItemVM> lookupDictionary = lookupSource
            .ToDictionary(x => x.Source);

         var sourceItems = this.GetValueNext<IEnumerable<TItemSource>>(
            context,
            ValueStage.None
         );

         try {
            collection.IsPopulating = true;

            collection.Clear();

            foreach (TItemSource sourceItem in sourceItems) {
               TItemVM vm;

               if (!lookupDictionary.TryGetValue(sourceItem, out vm)) {
                  throw new InvalidOperationException(ExceptionTexts.LookupViewModelNotFound);
               }

               collection.Add(vm);
            }
         } finally {
            collection.IsPopulating = false;
         }
      }
   }
}
