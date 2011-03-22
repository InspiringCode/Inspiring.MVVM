namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   /// <summary>
   ///   Counts the Add operations per item. The item is removed only when the count
   ///   of the Remove operations matches the count of the Add operations.
   /// </summary>
   public class CountingSet<TItem> : IEnumerable<TItem> {
      private readonly List<TItem> _items = new List<TItem>();
      private readonly List<KeyValuePair<TItem, int>> _numberedItems = new List<KeyValuePair<TItem, int>>();

      public void Add(TItem item) {
         int index = _numberedItems.FindIndex(x => x.Key.Equals(item));
         if (index < 0) {
            _numberedItems.Add(new KeyValuePair<TItem, int>(item, 1));
            _items.Add(item);
            return;
         }
         _numberedItems[index] = new KeyValuePair<TItem, int>(
            _numberedItems[index].Key,
            _numberedItems[index].Value + 1
         );
      }

      public void Remove(TItem item) {
         int index = _numberedItems.FindIndex(x => x.Key.Equals(item));
         if (index < 0) {
            return;
         }

         if (_numberedItems[index].Value > 1) {
            _numberedItems[index] = new KeyValuePair<TItem, int>(
               _numberedItems[index].Key,
               _numberedItems[index].Value - 1
            );
         } else {
            _numberedItems.RemoveAt(index);
            _items.Remove(item);
         }
      }

      public IEnumerator<TItem> GetEnumerator() {
         return _items.GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
         return _items.GetEnumerator();
      }
   }
}