namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class WrapperCollectionAccessorBehavior<TItemVM, TItemSource> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IValueAccessorBehavior<IEnumerable<TItemSource>>,
      IRefreshBehavior
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      private static readonly FieldDefinitionGroup CollectionSourceCacheGroup = new FieldDefinitionGroup();
      private readonly bool _shouldCacheSourceCollection;
      private DynamicFieldAccessor<IEnumerable<TItemSource>> _collectionSourceCache;

      public WrapperCollectionAccessorBehavior(bool shouldCacheSourceCollection) {
         _shouldCacheSourceCollection = shouldCacheSourceCollection;
      }

      IEnumerable<TItemSource> IValueAccessorBehavior<IEnumerable<TItemSource>>.GetValue(IBehaviorContext context) {
         return GetSourceItems(context);
      }

      void IValueAccessorBehavior<IEnumerable<TItemSource>>.SetValue(IBehaviorContext context, IEnumerable<TItemSource> value) {
         throw new NotSupportedException();
      }

      public override void Initialize(BehaviorInitializationContext context) {
         _collectionSourceCache = new DynamicFieldAccessor<IEnumerable<TItemSource>>(context, CollectionSourceCacheGroup);
         base.Initialize(context);
      }

      public override void SetValue(IBehaviorContext context, IVMCollection<TItemVM> value) {
         throw new NotSupportedException(
            ExceptionTexts.CannotSetVMCollectionProperties
         );
      }

      public void Refresh(IBehaviorContext context) {
         // Call next behavior first, because a source accessor behavior may handle
         // it.
         this.RefreshNext(context);

         _collectionSourceCache.Clear(context);
         var collection = GetValue(context);

         Dictionary<TItemSource, TItemVM> previousItemsBySource = collection.ToDictionary(
            x => x.Source,
            new ReferenceEqualityComparer<TItemSource>()
         );

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
            .ForEach(x => x.Item.Kernel.RefreshWithoutValidation());
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
         if (!_shouldCacheSourceCollection) {
            IEnumerable<TItemSource> source = this.GetValueNext<IEnumerable<TItemSource>>(context);
            return source;
         }

         if (!_collectionSourceCache.HasValue(context)) {
            IEnumerable<TItemSource> source = this.GetValueNext<IEnumerable<TItemSource>>(context);
            _collectionSourceCache.Set(context, source);
         }

         return _collectionSourceCache.Get(context);
      }

      private TItemVM CreateAndInitializeItem(IBehaviorContext context, TItemSource source) {
         var vm = this.CreateValueNext<TItemVM>(context);
         vm.Source = source;
         return vm;
      }
   }
}
