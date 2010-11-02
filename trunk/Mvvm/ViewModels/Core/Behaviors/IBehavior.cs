namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IBehavior {
      IBehavior Successor { get; set; }

      void Initialize(BehaviorInitializationContext context);
   }

   public class BehaviorInitializationContext {
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
