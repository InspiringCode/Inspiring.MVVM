namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   public sealed class PropertyBehaviorInitializationContext {
      public PropertyBehaviorInitializationContext(
         FieldDefinitionCollection fields,
         IVMProperty property
      ) {
         Fields = fields;
         Property = property;
      }

      public IVMProperty Property { get; private set; }

      public FieldDefinitionCollection Fields { get; private set; }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(Property != null);
         Contract.Invariant(Fields != null);
      }
   }
}
