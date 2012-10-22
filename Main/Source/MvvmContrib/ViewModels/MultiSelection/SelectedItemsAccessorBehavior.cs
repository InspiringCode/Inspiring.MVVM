namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class SelectedItemsAccessorBehavior<TVM, TItemVM, TItemSource> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IRefreshBehavior
      where TVM : IViewModel
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public void Refresh(IBehaviorContext context, RefreshOptions options) {
         var collection = GetValue(context);
         Repopulate(context, collection, RefreshReason.Create(options.ExecuteRefreshDependencies));

         this.RefreshNext(context, options);
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
         Repopulate(context, collection, InitialPopulationChangeReason.Instance);

         base.OnInitialize(context);
      }

      private void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection, IChangeReason reason) {
         var sourceItems = this.GetValueNext<IEnumerable<TItemSource>>(context);
         
         var items = sourceItems
            .Select(sourceItem => GetItemVM(context, sourceItem));
         
         collection.ReplaceItems(items, reason);
      }

      private TItemVM GetItemVM(IBehaviorContext context, TItemSource source) {
         var cacheBehavior = context
            .VM
            .Descriptor
            .Behaviors
            .GetNextBehavior<SelectionItemViewModelCacheBehavior<TItemSource, TItemVM>>();

         return cacheBehavior.GetVMForSource(context, source);
      }
   }
}
