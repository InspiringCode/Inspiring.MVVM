namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class LookupPopulatorCollectionBehavior<TVM, TItemVM, TItemSource> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IRefreshBehavior
      where TVM : IViewModel
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      private Func<TVM, IEnumerable<TItemVM>> _lookupSourceProvider;

      public LookupPopulatorCollectionBehavior(Func<TVM, IEnumerable<TItemVM>> lookupSourceProvider) {
         Contract.Requires(lookupSourceProvider != null);
         _lookupSourceProvider = lookupSourceProvider;
      }

      public void Refresh(IBehaviorContext context) {
         var collection = GetValue(context);
         Repopulate(context, collection);

         this.RefreshNext(context);
      }

      protected override IVMCollection<TItemVM> ProvideValue(IBehaviorContext context) {
         return this.CreateValueNext<IVMCollection<TItemVM>>(context);
      }

      protected override void OnInitialize(IBehaviorContext context) {
         // We have to populate the collection here and NOT in the ProvideValue
         // method to avoid the following endless recursion:
         //   1. GetValue call ProvideValue.
         //   2. ProvideValue populates the collection, which raises a change event.
         //   3. The change event triggers a view model level validation.
         //   4. The view model level validation may access the property again
         //      which calls GetValue and ProvideValue.

         var collection = this.GetValue(context);
         Repopulate(context, collection);

         base.OnInitialize(context);
      }


      private void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
         IEnumerable<TItemVM> lookupSource = _lookupSourceProvider((TVM)context.VM);

         Dictionary<TItemSource, TItemVM> lookupDictionary = lookupSource
            .ToDictionary(x => x.Source);

         var sourceItems = this.GetValueNext<IEnumerable<TItemSource>>(context);

         var items = sourceItems.Select(sourceItem => {
            TItemVM vm;

            if (!lookupDictionary.TryGetValue(sourceItem, out vm)) {
               throw new InvalidOperationException(
                  ExceptionTexts.LookupViewModelNotFound.FormatWith(sourceItem)
               );
            }

            return vm;
         });

         collection.ReplaceItems(items);
      }
   }
}
