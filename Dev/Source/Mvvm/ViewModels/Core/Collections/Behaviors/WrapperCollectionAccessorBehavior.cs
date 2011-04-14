namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class WrapperCollectionAccessorBehavior<TItemVM, TItemSource> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IRefreshBehavior
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      protected override IVMCollection<TItemVM> ProvideValue(IBehaviorContext context) {
         var collection = this.CreateValueNext<IVMCollection<TItemVM>>(context);
         Repopulate(context, collection);
         return collection;
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
