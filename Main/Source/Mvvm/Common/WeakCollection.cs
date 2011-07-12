namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using System.Runtime.InteropServices;

   /// <summary>
   ///   A collection of weak references to objects of type <typeparamref name="T"/>. 
   ///   If an item is not referenced somewhere else anymore, it is available for 
   ///   garbage collection and is removed from this collection as soon as it is
   ///   collected.
   /// </summary>
   /// <remarks>
   ///   More info can be found on the following sites:
   ///      http://connectedproperties.codeplex.com/SourceControl/changeset/view/d75e91b4b73e
   ///      http://www.codeproject.com/KB/cs/WeakReferencesGCHandles.aspx
   ///      http://nitoprograms.blogspot.com/2009/07/nitoweakreference-and.html
   /// </remarks>
   internal sealed class WeakCollection<T> : ICollection<T>, IDisposable {
      internal const int PurgeThreshold = 64;
      private List<GCHandle> _weakHandles = new List<GCHandle>();
      private bool _disposed = false;

      ~WeakCollection() {
         Dispose(false);
      }

      /// <inheritdoc />
      public int Count {
         get {
            CheckDisposed();

            // Count() would cause a endless recursion!
            return this.Count(x => true);
         }
      }

      /// <inheritdoc />
      bool ICollection<T>.IsReadOnly {
         get {
            CheckDisposed();
            return false;
         }
      }

      /// <inheritdoc />
      public void Add(T item) {
         CheckDisposed();

         if (item == null) {
            throw new ArgumentNullException(
               "item",
               ExceptionTexts.WeakCollectionDoesNotSupportNullItems
            );
         }

         _weakHandles.Add(GCHandle.Alloc(item, GCHandleType.Weak));
      }

      /// <inheritdoc />
      public void Clear() {
         CheckDisposed();

         DisposeHandles();
         _weakHandles.Clear();
      }

      /// <inheritdoc />
      public bool Remove(T item) {
         CheckDisposed();

         int i = _weakHandles.FindIndex(h => Object.Equals(h.Target, item));

         if (i == -1) {
            return false;
         }

         GCHandle weakHandle = _weakHandles[i];
         weakHandle.Free();

         _weakHandles.RemoveAt(i);
         return true;
      }

      /// <summary>
      ///   Enumerates all alive items.
      /// </summary>
      public IEnumerator<T> GetEnumerator() {
         CheckDisposed();

         int deadItems = 0;
         foreach (GCHandle handle in _weakHandles) {
            // Cache because 'Target' may change!
            object target = handle.Target;

            if (target != null) {
               yield return (T)target;
            } else {
               deadItems++;
            }
         }

         if (deadItems >= PurgeThreshold) {
            Purge();
         }
      }

      /// <inheritdoc />
      public void Dispose() {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      /// <inheritdoc />
      bool ICollection<T>.Contains(T item) {
         CheckDisposed();
         return this.Any(x => Object.Equals(x, item));
      }

      /// <inheritdoc />
      IEnumerator IEnumerable.GetEnumerator() {
         CheckDisposed();
         return GetEnumerator();
      }

      /// <inheritdoc />
      void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
         CheckDisposed();

         // Notes:
         //  (1) The items and the count cannot be accessed sequentially because items
         //      may die in the meantime.
         //  (2) Using the ICollection<T>.CopyTo implementation of List<T> automatically 
         //      implements the correct contract (exceptions, etc.) for us.
         //  (3) We can not use LINQ ToList or new List(this) because they use CopyTo
         //      internally.
         ICollection<T> liveItems = new List<T>();
         foreach (T item in this) {
            liveItems.Add(item);
         }

         liveItems.CopyTo(array, arrayIndex);
      }

      /// <remarks>
      ///   <para>This implementation uses logic similar to List<T>.RemoveAll, which 
      ///      always has O(n) time. If we would simply call Remove in a loop, the running 
      ///      time would approach O(n^2) if most items die (mass distinction).</para>
      ///   <para>Note that we can not easily use <see cref="List{T}.RemoveAll"/> because
      ///      we have to do the check, wheter an item is still alive only once in total.
      ///      This condition is needed to dertmine (1) if the item should be removed and
      ///      (2) if the corresponding handle should be freed. Doing the check twice may
      ///      lead to an inconsistent state or memory leaks because the object may die
      ///      between the two checks.</para>   
      /// </remarks>
      private void Purge() {
         int writeIndex = 0;
         for (int readIndex = 0; readIndex < _weakHandles.Count; readIndex++) {
            GCHandle handle = _weakHandles[readIndex];

            bool itemStillAlive = handle.Target != null;
            if (itemStillAlive) {
               if (readIndex != writeIndex) {
                  _weakHandles[writeIndex] = _weakHandles[readIndex];
               }

               writeIndex++;
            } else {
               handle.Free();
            }
         }

         _weakHandles.RemoveRange(writeIndex, _weakHandles.Count - writeIndex);
      }

      private void Dispose(bool disposing) {
         if (!_disposed) {
            DisposeHandles();
            _disposed = true;
         }
      }

      private void DisposeHandles() {
         if (_weakHandles == null) {
            return;
         }

         foreach (GCHandle handle in _weakHandles) {
            if (handle.IsAllocated) {
               handle.Free();
            }
         }
      }

      private void CheckDisposed() {
         if (_disposed) {
            throw new ObjectDisposedException(ToString());
         }
      }
   }
}