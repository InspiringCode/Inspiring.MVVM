namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   public sealed class BehaviorInitializationContext {
      public BehaviorInitializationContext(
         VMDescriptorBase descriptor,
         IVMProperty property = null
      ) {
         Contract.Requires(descriptor != null);

         Fields = descriptor.Fields;
         Descriptor = descriptor;
         Property = property;
      }

      public FieldDefinitionCollection Fields { get; private set; }

      public VMDescriptorBase Descriptor { get; private set; }

      public IVMProperty Property { get; private set; }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(Fields != null);
         Contract.Invariant(Descriptor != null);
      }
   }
}
