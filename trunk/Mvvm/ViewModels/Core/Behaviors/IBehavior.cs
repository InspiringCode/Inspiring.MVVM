namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IBehavior {
      IBehavior Successor { get; set; }

      void Initialize(BehaviorInitializationContext context);
   }

   public class BehaviorInitializationContext {
      internal BehaviorInitializationContext(VMProperty property) {
         DynamicFields = property.Descriptor.DynamicFields;
         Property = property;
      }

      //internal BehaviorInitializationContext(
      //   FieldDefinitionCollection dynamicFields,
      //   VMProperty property
      //) {
      //   PropertyName = propertyName;
      //   DynamicFields = dynamicFields;
      //}

      public FieldDefinitionCollection DynamicFields { get; private set; }

      public VMProperty Property { get; private set; }
   }
}
