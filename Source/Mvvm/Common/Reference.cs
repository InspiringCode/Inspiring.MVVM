namespace Inspiring.Mvvm.Common {
   using System;

   internal sealed class Reference<T> {
      public Reference() {
      }

      public Reference(T initialValue) {
         Value = initialValue;
      }

      public T Value {
         get;
         set;
      }

      public void Replace(Func<T, T> factory) {
         Value = factory(Value);
      }
   }
}
