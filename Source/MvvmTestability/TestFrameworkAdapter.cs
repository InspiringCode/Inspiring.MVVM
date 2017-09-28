namespace Inspiring.Mvvm.Testability {
   using System;
   
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
         Check.NotNull(adapter, nameof(adapter));
         _current = adapter;
      }
   }
}
