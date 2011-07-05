namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

   public abstract class VMDescriptorBase : IVMDescriptor {
      private VMPropertyCollection _properties;
      private readonly FieldDefinitionCollection _fields;

      public VMDescriptorBase() {
         _fields = new FieldDefinitionCollection();
         Behaviors = new BehaviorChain();
      }

      public FieldDefinitionCollection Fields {
         get {
            Contract.Ensures(Contract.Result<FieldDefinitionCollection>() != null);
            return _fields;
         }
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

      public BehaviorChain Behaviors { get; set; }

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
