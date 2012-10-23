namespace Inspiring.Mvvm.Testability {
   using System;
   using System.Diagnostics.Contracts;

   public static class TestFrameworkAdapter {
      private static ITestFrameworkAdapter _current;

      internal static ITestFrameworkAdapter Current {
         get {
            if (_current == null) {
               throw new InvalidOperationException(ExceptionTexts.NoTestFrameworkAdapterConfigured);
            }
            return _current;
         }
      }

      public static void SetTestFrameworkAdapter(ITestFrameworkAdapter adapter) {
         Contract.Requires<ArgumentNullException>(adapter != null);
         _current = adapter;
      }
   }
}
