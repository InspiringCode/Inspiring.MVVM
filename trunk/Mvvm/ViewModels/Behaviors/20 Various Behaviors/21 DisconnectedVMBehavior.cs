namespace Inspiring.Mvvm.ViewModels.Behaviors {
   internal sealed class DisconnectedVMBehavior<TValue> : VMPropertyBehavior, IDisconnectedVMBehavior<TValue> {
      FieldDefinition<TValue> _localCopyField;

      public void CopyFromSource(IBehaviorContext vm) {
         AssertInitialized();
         IAccessPropertyBehavior<TValue> accessBehavior = GetNextBehavior<IAccessPropertyBehavior<TValue>>();
         TValue sourceValue = accessBehavior.GetValue(vm);
         vm.FieldValues.SetValue(_localCopyField, sourceValue);
      }

      public void CopyToSource(IBehaviorContext vm) {
         AssertInitialized();
         IAccessPropertyBehavior<TValue> accessBehavior = GetNextBehavior<IAccessPropertyBehavior<TValue>>();
         TValue localValue = vm.FieldValues.GetValue(_localCopyField);
         accessBehavior.SetValue(vm, localValue);
      }

      public TValue GetValue(IBehaviorContext vm) {
         AssertInitialized();
         return vm.FieldValues.GetValue(_localCopyField);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         AssertInitialized();
         vm.FieldValues.SetValue(_localCopyField, value);
      }
      protected override void OnDefineDynamicFields(FieldDefinitionCollection fields) {
         _localCopyField = fields.DefineField<TValue>(DynamicFieldGroups.LocalCopyGroup);
      }
   }
}
