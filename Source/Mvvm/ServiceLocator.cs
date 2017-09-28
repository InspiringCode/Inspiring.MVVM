namespace Inspiring.Mvvm {
   using System;

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
         Check.NotNull(locator, nameof(locator));
         _current = locator;
      }
   }
}
