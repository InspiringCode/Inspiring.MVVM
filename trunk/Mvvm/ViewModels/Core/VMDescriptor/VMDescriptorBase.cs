namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Core;

   public abstract class VMDescriptorBase : ServiceRegistry {
      private VMPropertyCollection _properties;

      public VMDescriptorBase() {
         Behaviors = new Behavior();
      }

      public VMPropertyCollection Properties {
         get {
            if (_properties == null) {
               _properties = DiscoverProperties();
               Seal();
            }
            return _properties;
         }
      }

      public Behavior Behaviors { get; private set; }

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
