namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   // TODO: Rename to BehaviorInitializationContext
   public sealed class InitializationContext {
      public InitializationContext(
         FieldDefinitionCollection fields,
         IVMProperty property = null
      ) {
         Fields = fields;
         Property = property;
      }

      public IVMProperty Property { get; private set; }

      public FieldDefinitionCollection Fields { get; private set; }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(Fields != null);
      }
   }
}
