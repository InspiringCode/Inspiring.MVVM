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
         IList<TItemSource> list = GetSourceList();
         list.Insert(index, sourceItem);
      }

      public void Remove(TItemVM item) {
         TItemSource sourceItem = GetSource(item);
         _sourceCollection.Remove(sourceItem);
      }

      public void SetItem(TItemVM item, int index) {
         TItemSource sourceItem = GetSource(item);
         IList<TItemSource> list = GetSourceList();
         list[index] = sourceItem;
      }

      public void Clear() {
         _sourceCollection.Clear();
      }

      private TItemSource GetSource(TItemVM vm) {
         IHasSourceObject<TItemSource> withSource = vm as IHasSourceObject<TItemSource>;
         if (vm == null) {
            throw new InvalidOperationException(
               ExceptionTexts.HasSourceObjectInterfaceNotImplemented
            );
         }
         return withSource.Source;
      }

      private IList<TItemSource> GetSourceList() {
         IList<TItemSource> list = _sourceCollection as IList<TItemSource>;
         if (list == null) {
            throw new InvalidOperationException(
               ExceptionTexts.CollectionSourceDoesNotImplementListInterface
            );
         }
         return list;
      }
   }
}
