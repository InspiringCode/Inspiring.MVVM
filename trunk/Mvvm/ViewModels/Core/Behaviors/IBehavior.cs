namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IBehavior {
      IBehavior Successor { get; set; }

      void Initialize(BehaviorInitializationContext context);
   }

   public class BehaviorInitializationContext {
      internal BehaviorInitializationContext(VMPropertyBase property) {
         DynamicFields = property.Descriptor.DynamicFields;
         Property = property;
      }

      internal BehaviorInitializationContext(
         FieldDefinitionCollection dynamicFields,
         VMPropertyBase property
      ) {
         Property = property;
         DynamicFields = dynamicFields;
      }

      public FieldDefinitionCollection DynamicFields { get; private set; }

      public VMPropertyBase Property { get; private set; }
   }
}
