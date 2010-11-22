namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class VMPropertyCollection :
      IEnumerable<IVMProperty> {

      private IVMProperty[] _properties;

      public VMPropertyCollection(IVMProperty[] properties) {
         Contract.Requires<ArgumentNullException>(properties != null);

         _properties = properties;
      }

      public IVMProperty this[string propertyName] {
         get {
            IVMProperty property;
            bool found = TryGetProperty(propertyName, out property);

            if (!found) {
               throw new ArgumentException(
                  ExceptionTexts.PropertyNotFound.FormatWith(propertyName)
               );
            }

            return property;
         }
      }

      public bool TryGetProperty(string propertyName, out IVMProperty property) {
         Contract.Requires<ArgumentNullException>(propertyName != null);

         property = Array.Find(_properties, p => p.PropertyName == propertyName);
         return property != null;
      }

      public IEnumerator<IVMProperty> GetEnumerator() {
         IEnumerable<IVMProperty> typed = _properties;
         return typed.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return _properties.GetEnumerator();
      }
   }
}
