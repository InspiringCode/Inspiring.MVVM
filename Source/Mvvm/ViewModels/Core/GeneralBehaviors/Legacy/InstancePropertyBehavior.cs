namespace Inspiring.Mvvm.ViewModels.Core {

   /// <summary>
   ///   An <see cref="IDisplayValueAccessorBehavior"/> that implements the get/set 
   ///   operation of a <see cref="IVMPropertyDescriptor"/> by using a dynamic field (see 
   ///   <see cref="FieldDefinition"/>) as a backing store for the property target.
   /// </summary>
   public sealed class InstancePropertyBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue> {

      private FieldDefinition<TValue> _backingField;

      public TValue GetValue(IBehaviorContext vm) {
         return vm.FieldValues.GetValueOrDefault(_backingField);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         vm.FieldValues.SetValue(_backingField, value);
      }

      public void Initialize(BehaviorInitializationContext context) {
         _backingField = context.Fields.DefineField<TValue>(
            DynamicFieldGroups.BackingFieldGroup
         );

         this.InitializeNext(context);
      }
   }
}
