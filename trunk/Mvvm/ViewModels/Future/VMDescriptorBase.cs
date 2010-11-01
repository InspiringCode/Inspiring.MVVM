namespace Inspiring.Mvvm.ViewModels.Future {
   using System;
   using Inspiring.Mvvm.Common;

   public abstract class VMDescriptorBase : ServiceRegistry {
      private VMPropertyCollection _properties;
      private bool _isSealed;

      public VMPropertyCollection Properties {
         get {
            if (_properties == null) {
               _properties = DiscoverProperties();
               Seal();
            }
            return _properties;
         }
      }

      public void Modify() {
         if (_isSealed) {
            throw new InvalidOperationException(ExceptionTexts.ObjectIsSealed);
         }
      }

      public void Seal() {
         _isSealed = true;
      }

      protected abstract VMPropertyCollection DiscoverProperties();
   }
}
