namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IBehavior {
      IBehavior Successor { get; set; }

      void Initialize(BehaviorInitializationContext context);
   }

   public class BehaviorInitializationContext {
      internal BehaviorInitializationContext(VMProperty property)
         : this(property.PropertyName, property.Descriptor.DynamicFields) {
      }

      internal BehaviorInitializationContext(
         string propertyName,
         FieldDefinitionCollection dynamicFields
      ) {
         PropertyName = propertyName;
         DynamicFields = dynamicFields;
      }


      public string PropertyName { get; private set; }

      public FieldDefinitionCollection DynamicFields { get; private set; }
   }
}
