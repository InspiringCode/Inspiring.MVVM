namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   internal sealed class CollectionModificationController<TItemVM, TItemSource> :
      ICollectionModificationController<TItemVM>
      where TItemVM : ViewModel {

      private ICollection<TItemSource> _sourceCollection;

      public CollectionModificationController(
         ICollection<TItemSource> collectionSource
      ) {
         _sourceCollection = collectionSource;
      }

      public void Insert(TItemVM item, int index) {
         TItemSource sourceItem = GetSource(item);
         IList<TItemSource> sourceList = _sourceCollection as IList<TItemSource>;
         if (sourceList != null) {
            sourceList.Insert(index, sourceItem);
         } else {
            // We simply add the item if the underlying collection does not
            // support Inserts (e.g. if it is a Set).
            _sourceCollection.Add(sourceItem);
         }
      }

      public void Remove(TItemVM item) {
         TItemSource sourceItem = GetSource(item);
         _sourceCollection.Remove(sourceItem);
      }

      public void SetItem(TItemVM item, int index) {
         TItemSource sourceItem = GetSource(item);

         IList<TItemSource> list = _sourceCollection as IList<TItemSource>;
         if (list == null) {
            throw new InvalidOperationException(
               ExceptionTexts.CollectionSourceDoesNotImplementListInterface
            );
         }

         list[index] = sourceItem;
      }

      public void Clear() {
         _sourceCollection.Clear();
      }

      private TItemSource GetSource(TItemVM vm) {
         IVMCollectionItem<TItemSource> withSource = vm as IVMCollectionItem<TItemSource>;
         if (withSource == null) {
            throw new InvalidOperationException(
               ExceptionTexts.HasSourceObjectInterfaceNotImplemented
            );
         }
         return withSource.Source;
      }
   }
}
