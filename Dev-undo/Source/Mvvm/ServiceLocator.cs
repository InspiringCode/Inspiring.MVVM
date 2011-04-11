namespace Inspiring.Mvvm {
   using System;
   using System.Diagnostics.Contracts;

   public static class ServiceLocator {
      private static IServiceLocator _current;

      internal static IServiceLocator Current {
         get {
            if (_current == null) {
               throw new InvalidOperationException(ExceptionTexts.NoServiceLocatorConfigured);
            }
            return _current;
         }
      }

      public static void SetServiceLocator(IServiceLocator locator) {
         Contract.Requires<ArgumentNullException>(locator != null);
         _current = locator;
      }
   }
}
