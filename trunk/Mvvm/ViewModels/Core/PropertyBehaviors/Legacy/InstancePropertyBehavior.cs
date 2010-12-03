namespace Inspiring.Mvvm.ViewModels.Core {

   /// <summary>
   ///   An <see cref="IDisplayValueAccessorBehavior"/> that implements the get/set 
   ///   operation of a <see cref="VMPropertyBase"/> by using a dynamic field (see 
   ///   <see cref="FieldDefinition"/>) as a backing store for the property target.
   /// </summary>
   public sealed class InstancePropertyBehavior<TValue> : Behavior, IValueAccessorBehavior<TValue> {
      private FieldDefinition<TValue> _backingField;

      public TValue GetValue(IBehaviorContext vm, ValueStage stage) {
         return vm.FieldValues.GetValueOrDefault(_backingField);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         vm.FieldValues.SetValue(_backingField, value);
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _backingField = context.Fields.DefineField<TValue>(
            DynamicFieldGroups.BackingFieldGroup
         );
      }
   }
}
