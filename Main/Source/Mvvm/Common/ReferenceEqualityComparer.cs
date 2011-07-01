namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Runtime.CompilerServices;

   internal sealed class ReferenceEqualityComparer<T> : EqualityComparer<T> {
      public override bool Equals(T x, T y) {
         return Object.ReferenceEquals(x, y);
      }

      public override int GetHashCode(T obj) {
         return RuntimeHelpers.GetHashCode(obj);
      }
   }
}
