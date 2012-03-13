namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Collections;

   public abstract class AbstractCollection<T> : ICollection<T> {
      private readonly ICollection<T> _inner;

      internal AbstractCollection(ICollection<T> inner) {
         _inner = inner;
      }

      int ICollection<T>.Count {
         get { return _inner.Count; }
      }

      bool ICollection<T>.IsReadOnly {
         get { return _inner.IsReadOnly; }
      }

      void ICollection<T>.Add(T item) {
         AddCore(item);
      }

      bool ICollection<T>.Remove(T item) {
         return RemoveCore(item);
      }

      void ICollection<T>.Clear() {
         _inner.Clear();
      }

      bool ICollection<T>.Contains(T item) {
         return _inner.Contains(item);
      }

      void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
         _inner.CopyTo(array, arrayIndex);
      }
      
      public IEnumerator<T> GetEnumerator() {
         return _inner.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return _inner.GetEnumerator();
      }

      protected virtual void AddCore(T item) {
         _inner.Add(item);
      }

      protected virtual bool RemoveCore(T item) {
         return _inner.Remove(item);
      }
   }
}
