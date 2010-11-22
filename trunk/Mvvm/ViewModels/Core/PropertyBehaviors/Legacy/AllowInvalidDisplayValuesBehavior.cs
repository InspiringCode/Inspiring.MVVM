namespace Inspiring.Mvvm.ViewModels.Core {

   /// <summary>
   ///   A behavior that allows the display value of a VMProperty to take on an
   ///   invalid value.
   /// </summary>
   /// <remarks>
   ///   <para>If the display value is set to a value that does not pass 
   ///   conversion or validation, the invalid value is cached and returned when
   ///   the display value is accessed. Once the property is set to a valid value 
   ///   again (either by setting its display value or strongly typed value), the
   ///   invalid value is discarded.</para>
   ///   <para>This behavior is useful in binding scenarios when the users enters 
   ///   an invalid value. Because WPF rereads the value after setting it on the 
   ///   VM, the bound element would be reset to its last valid value everytime 
   ///   the binding synchronizes.</para>
   /// </remarks>
   public sealed class AllowInvalidDisplayValuesBehavior :
      Behavior,
      IDisplayValueAccessorBehavior,
      IHandlePropertyChangingBehavior {

      private FieldDefinition<object> _invalidValueField;

      public object GetDisplayValue(IBehaviorContext vm) {
         object invalidValue;
         if (vm.FieldValues.TryGetValue(_invalidValueField, out invalidValue)) {
            return invalidValue;
         }

         return GetNextBehavior<IDisplayValueAccessorBehavior>().GetDisplayValue(vm);
      }

      public void SetDisplayValue(IBehaviorContext vm, object value) {
         vm.FieldValues.SetValue(_invalidValueField, value);
         GetNextBehavior<IDisplayValueAccessorBehavior>().SetDisplayValue(vm, value);
      }

      public void HandlePropertyChanging(IBehaviorContext vm) {
         // The value was set successfully on the source object: discard invalid
         // value and return the actual value next time!
         vm.FieldValues.ClearField(_invalidValueField);

         IHandlePropertyChangingBehavior next;
         if (TryGetBehavior(out next)) {
            next.HandlePropertyChanging(vm);
         }
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _invalidValueField = context.Fields.DefineField<object>(
            DynamicFieldGroups.InvalidValueGroup
         );
      }
   }
}
