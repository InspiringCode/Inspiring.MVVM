namespace Inspiring.Mvvm.Common {
   using System;

   internal struct Optional<T> {
      private T _value;
      private bool _hasValue;

      public Optional(T value) {
         _value = value;
         _hasValue = true;
      }

      public T Value {
         get {
            if (!HasValue) {
               throw new InvalidOperationException(ExceptionTexts.NoValue);
            }

            return _value;
         }
      }

      public bool HasValue {
         get {
            return _hasValue;
         }
      }
   }
}
