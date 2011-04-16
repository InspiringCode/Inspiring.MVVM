namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class WrapperCollectionAccessorBehavior<TItemVM, TItemSource> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IRefreshBehavior
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public override void SetValue(IBehaviorContext context, IVMCollection<TItemVM> value) {
         throw new NotSupportedException(
            ExceptionTexts.CannotSetVMCollectionProperties
         );
      }

      public void Refresh(IBehaviorContext context) {
         var collection = GetValue(context);
         var previousItemsBySource = collection.ToDictionary(x => x.Source);
         var newSourceItems = GetSourceItems(context);

         var newItems = newSourceItems.Select(s => {
            TItemVM item;

            bool isReusedItem = previousItemsBySource.TryGetValue(s, out item);

            if (!isReusedItem) {
               item = CreateAndInitializeItem(context, s);
            }

            return new { IsReusedItem = isReusedItem, Item = item };
         }).ToArray();

         collection.ReplaceItems(newItems.Select(x => x.Item));

         newItems
            .Where(x => x.IsReusedItem)
            .ForEach(x => x.Item.Kernel.Refresh());

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
         var sourceItems = GetSourceItems(context);

         IEnumerable<TItemVM> newItems = sourceItems
            .Select(s => CreateAndInitializeItem(context, s));

         collection.ReplaceItems(newItems);
      }

      private IEnumerable<TItemSource> GetSourceItems(IBehaviorContext context) {
         return this.GetValueNext<IEnumerable<TItemSource>>(context);
      }

      private TItemVM CreateAndInitializeItem(IBehaviorContext context, TItemSource source) {
         var vm = this.CreateValueNext<TItemVM>(context);
         vm.Source = source;
         return vm;
      }
   }
}
