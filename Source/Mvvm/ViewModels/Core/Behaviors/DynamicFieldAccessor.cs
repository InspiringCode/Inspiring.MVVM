namespace Inspiring.Mvvm.ViewModels.Core {

   // TODO: Is public a good idea? Should ONLY this class be public (and the fieldvalues, etc. internal?)
   public sealed class DynamicFieldAccessor<TValue> {
      private readonly FieldDefinition<TValue> _field;

      public DynamicFieldAccessor(
         BehaviorInitializationContext context,
         FieldDefinitionGroup fieldGroup
      ) {
         Check.NotNull(context, nameof(context));
         Check.NotNull(fieldGroup, nameof(fieldGroup));

         _field = context.Fields.DefineField<TValue>(fieldGroup);
      }

      public bool HasValue(IBehaviorContext context) {
         return context.FieldValues.HasValue(_field);
      }

      public bool TryGet(IBehaviorContext context, out TValue value) {
         return context.FieldValues.TryGetValue(_field, out value);
      }

      public TValue Get(IBehaviorContext context) {
         return context.FieldValues.GetValue(_field);
      }

      public TValue GetWithDefault(IBehaviorContext context, TValue defaultValue) {
         return context.FieldValues.GetValueOrDefault(_field, defaultValue);
      }

      public void Set(IBehaviorContext context, TValue value) {
         context.FieldValues.SetValue(_field, value);
      }

      public void Clear(IBehaviorContext context) {
         context.FieldValues.ClearField(_field);
      }
   }
}