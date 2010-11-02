namespace Inspiring.Mvvm.ViewModels.Future {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   public abstract class VMDescriptorBase : ServiceRegistry {
      private VMPropertyCollection _properties;

      public VMPropertyCollection Properties {
         get {
            if (_properties == null) {
               _properties = DiscoverProperties();
               Seal();
            }
            return _properties;
         }
      }

      public bool IsSealed { get; private set; }

      public void Modify() {
         Contract.Requires<InvalidOperationException>(
            !IsSealed,
            ExceptionTexts.ObjectIsSealed
         );
      }

      public void Seal() {
         IsSealed = true;
      }

      protected abstract VMPropertyCollection DiscoverProperties();
   }
}
