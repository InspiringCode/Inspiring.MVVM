namespace Inspiring.Mvvm.Common {
   using System;

   internal struct Optional<T> {
      private T _value;
      private bool _hasValue;

      public Optional(T value) {
         _value = default(T);
         _hasValue = default(bool);

         Value = value;
      }

      public bool HasValue {
         get { return _hasValue; }
      }

      public T Value {
         get {
            Check.Requires<InvalidOperationException>(HasValue);
            return _value;
         }
         set {
            _value = value;
            _hasValue = true;
         }
      }

      public void Clear() {
         _value = default(T);
         _hasValue = false;
      }

      public void Set(T value) {
         Value = value;
      }

      public override bool Equals(object obj) {
         if (!(obj is Optional<T>)) {
            return false;
         }

         Optional<T> other = (Optional<T>)obj;

         if (HasValue && other.HasValue) {
            return Object.Equals(Value, other.Value);
         }

         return HasValue == other.HasValue;
      }

      public override int GetHashCode() {
         return base.GetHashCode();
      }
   }
}
