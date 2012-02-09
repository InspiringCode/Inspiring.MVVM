namespace Inspiring.Mvvm.Common {
   using System;
   using System.Threading;
   using Inspiring.Mvvm.Common.Core;

   /// <summary>
   ///   Similar to a C# lock statement but works across processes.
   /// </summary>
   /// <example>
   ///   <code>
   ///      <![CDATA[using(new InterprocessLock("ExampleAppStartupSection", 10 * 1000) {
   ///         // Code whose execution should be serialized
   ///      }]]>
   ///   </code>
   /// </example>
   public sealed class InterprocessLock : IDisposable {
      private const int InfiniteTimeout = -1;

      private Mutex _mutex = null;

      public InterprocessLock(int aquisitionTimeoutMilliseconds = InfiniteTimeout)
         : this(UniqueAppKey.GetWithPrefix("lock:"), aquisitionTimeoutMilliseconds) {
      }

      public InterprocessLock(string lockName, int aquisitionTimeoutMilliseconds = InfiniteTimeout) {
         AquireMutex(lockName, TimeSpan.FromMilliseconds(aquisitionTimeoutMilliseconds));
      }

      /// <summary>
      ///   Releases the lock. This method must be called from the same thread
      ///   that created this object.
      /// </summary>
      public void Dispose() {
         if (_mutex != null) {
            try {
               _mutex.ReleaseMutex();
            } finally {
               _mutex = null;
            }
         }
      }

      private void AquireMutex(string mutexName, TimeSpan timeout) {
         _mutex = new Mutex(false, mutexName);

         try {
            bool aquired = _mutex.WaitOne(timeout);
            bool timeoutOccured = !aquired;

            if (timeoutOccured) {
               throw new TimeoutException();
            }
         } catch (AbandonedMutexException) {
            _mutex.ReleaseMutex();
            _mutex = null;

            AquireMutex(mutexName, timeout);
         }
      }
   }
}
