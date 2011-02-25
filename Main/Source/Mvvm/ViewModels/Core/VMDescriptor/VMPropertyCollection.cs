namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class VMPropertyCollection :
      IEnumerable<IVMPropertyDescriptor> {

      private IVMPropertyDescriptor[] _properties;

      public VMPropertyCollection(IVMPropertyDescriptor[] properties) {
         Contract.Requires<ArgumentNullException>(properties != null);

         _properties = properties;
      }

      public IVMPropertyDescriptor this[string propertyName] {
         get {
            IVMPropertyDescriptor property;
            bool found = TryGetProperty(propertyName, out property);

            if (!found) {
               throw new ArgumentException(
                  ExceptionTexts.PropertyNotFound.FormatWith(propertyName)
               );
            }

            return property;
         }
      }

      public bool TryGetProperty(string propertyName, out IVMPropertyDescriptor property) {
         Contract.Requires<ArgumentNullException>(propertyName != null);

         property = Array.Find(_properties, p => p.PropertyName == propertyName);
         return property != null;
      }

      public IEnumerator<IVMPropertyDescriptor> GetEnumerator() {
         IEnumerable<IVMPropertyDescriptor> typed = _properties;
         return typed.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return _properties.GetEnumerator();
      }
   }
}
