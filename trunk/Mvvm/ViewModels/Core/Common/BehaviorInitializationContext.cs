namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   public sealed class BehaviorInitializationContext {
      // TODO: Maybe refactor this to take only the descriptor?
      public BehaviorInitializationContext(
         FieldDefinitionCollection fields,
         VMDescriptorBase descriptor
      ) {
         Contract.Requires(fields != null);
         Contract.Requires(descriptor != null);

         Fields = fields;
         Descriptor = descriptor;
      }

      public BehaviorInitializationContext(
         FieldDefinitionCollection fields,
         VMDescriptorBase descriptor,
         IVMProperty property
      )
         : this(fields, descriptor) {
         Contract.Requires(fields != null);
         Contract.Requires(descriptor != null);
         Contract.Requires(property != null);

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
