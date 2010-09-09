namespace Inspiring.Mvvm.ViewModels.Behaviors {

   /// <summary>
   ///   An <see cref="IAccessPropertyBehavior"/> that implements the get/set 
   ///   operation of a <see cref="VMProperty"/> by using a dynamic field (see 
   ///   <see cref="FieldDefinition"/>) as a backing store for the property target.
   /// </summary>
   public sealed class InstancePropertyBehavior<TValue> : VMPropertyBehavior, IAccessPropertyBehavior<TValue> {
      private FieldDefinition<TValue> _backingField;

      public override BehaviorPosition Position {
         get { return BehaviorPosition.SourceValueAccessor; }
      }

      public TValue GetValue(IBehaviorContext vm) {
         AssertInitialized();
         return vm.FieldValues.GetValueOrDefault(_backingField);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         AssertInitialized();
         vm.FieldValues.SetValue(_backingField, value);
      }

      protected override void OnDefineDynamicFields(FieldDefinitionCollection fields) {
         base.OnDefineDynamicFields(fields);
         _backingField = fields.DefineField<TValue>(DynamicFieldGroups.BackingFieldGroup);
      }
   }
}
