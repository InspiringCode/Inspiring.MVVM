namespace Inspiring.Mvvm.ViewModels.Future {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class VMPropertyCollection : IEnumerable<VMPropertyBase> {
      private VMPropertyBase[] _properties;

      public VMPropertyCollection(VMPropertyBase[] properties) {
         Contract.Requires<ArgumentNullException>(properties != null);

         _properties = properties;
      }

      public VMPropertyBase this[string propertyName] {
         get {
            VMPropertyBase property;
            bool found = TryGetProperty(propertyName, out property);

            if (!found) {
               throw new ArgumentException(
                  ExceptionTexts.PropertyNotFound.FormatWith(propertyName)
               );
            }

            return property;
         }
      }

      public bool TryGetProperty(string propertyName, out VMPropertyBase property) {
         Contract.Requires<ArgumentNullException>(propertyName != null);

         property = Array.Find(_properties, p => p.PropertyName == propertyName);
         return property != null;
      }

      public IEnumerator<VMPropertyBase> GetEnumerator() {
         IEnumerable<VMPropertyBase> typed = _properties;
         return typed.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return _properties.GetEnumerator();
      }
   }
}
