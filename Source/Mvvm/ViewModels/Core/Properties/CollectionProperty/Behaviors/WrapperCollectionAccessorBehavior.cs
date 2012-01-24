namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   // This class is public to allow the very advanced user to override the equality comparer
   // used to determine reusability of item VMs.

   public sealed class WrapperCollectionAccessorBehavior<TItemVM, TItemSource> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IValueAccessorBehavior<IEnumerable<TItemSource>>,
      IRefreshBehavior
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      private static readonly FieldDefinitionGroup CollectionSourceCacheGroup = new FieldDefinitionGroup();

      private readonly bool _shouldCacheSourceCollection;
      private readonly IEqualityComparer<TItemSource> _reusabilitySourceComparer;

      private DynamicFieldAccessor<IEnumerable<TItemSource>> _collectionSourceCache;

      public WrapperCollectionAccessorBehavior(
         bool shouldCacheSourceCollection,
         IEqualityComparer<TItemSource> reusabilitySourceComparer = null
      ) {
         _shouldCacheSourceCollection = shouldCacheSourceCollection;

         _reusabilitySourceComparer =
            reusabilitySourceComparer ??
            ReferenceEqualityComparer<TItemSource>.CreateSmartComparer();
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

      public void Refresh(IBehaviorContext context, RefreshOptions options) {
         // Call next behavior first, because a source accessor behavior may handle
         // it.
         this.RefreshNext(context, options);
         _collectionSourceCache.Clear(context);

         // ToArray so that (1) source is only enumerated once and (2) acess by index
         // (required by the equality check) is guaranteed to be fast.
         TItemSource[] newSourceItems = GetSourceItems(context).ToArray();
         IVMCollection<TItemVM> vmCollection = GetValue(context);

         IEnumerable<TItemVM> itemsToRefresh = Enumerable.Empty<TItemVM>();

         if (AreCollectionContentsEqual(vmCollection, newSourceItems)) {
            itemsToRefresh = vmCollection;
         } else {
            Dictionary<TItemSource, TItemVM> previousItemsBySource = vmCollection.ToDictionary(
               x => x.Source,
               _reusabilitySourceComparer
            );

            List<TItemVM> newItems = new List<TItemVM>();
            List<TItemVM> reusedItems = new List<TItemVM>();

            foreach (TItemSource s in newSourceItems) {
               TItemVM item;
               bool isReusedItem = previousItemsBySource.TryGetValue(s, out item);

               if (isReusedItem) {
                  reusedItems.Add(item);
               } else {
                  item = CreateAndInitializeItem(context, s);
               }

               newItems.Add(item);
            }

            vmCollection.ReplaceItems(newItems, RefreshReason.Create(options.ExecuteRefreshDependencies));
            itemsToRefresh = reusedItems;
         }

         if (options.Scope.HasFlag(RefreshScope.Content)) {
            itemsToRefresh
               .ForEach(x => x.Kernel.RefreshWithoutValidation(options.ExecuteRefreshDependencies));
         }
      }

      private bool AreCollectionContentsEqual(
         IVMCollection<TItemVM> vmCollection,
         TItemSource[] sourceCollection
      ) {
         if (sourceCollection.Length != vmCollection.Count) {
            return false;
         }

         for (int i = 0; i < sourceCollection.Length; i++) {
            if (!_reusabilitySourceComparer.Equals(sourceCollection[i], vmCollection[i].Source)) {
               return false;
            }
         }

         return true;
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
         var sourceItems = GetSourceItems(context);

         IEnumerable<TItemVM> newItems = sourceItems
            .Select(s => CreateAndInitializeItem(context, s));

         collection.ReplaceItems(newItems, reason);
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
