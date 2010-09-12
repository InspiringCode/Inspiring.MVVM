namespace Inspiring.Mvvm.ViewModels.Core {
   internal sealed class CacheValueBehavior<TValue> : Behavior, ICacheValueBehavior<TValue> {
      FieldDefinition<TValue> _localCopyField;

      public void CopyFromSource(IBehaviorContext vm) {
         IAccessPropertyBehavior<TValue> accessBehavior = GetNextBehavior<IAccessPropertyBehavior<TValue>>();
         TValue sourceValue = accessBehavior.GetValue(vm);
         vm.FieldValues.SetValue(_localCopyField, sourceValue);
      }

      public void CopyToSource(IBehaviorContext vm) {
         IAccessPropertyBehavior<TValue> accessBehavior = GetNextBehavior<IAccessPropertyBehavior<TValue>>();
         TValue localValue = vm.FieldValues.GetValue(_localCopyField);
         accessBehavior.SetValue(vm, localValue);
      }

      public TValue GetValue(IBehaviorContext vm) {
         if (!vm.FieldValues.HasValue(_localCopyField)) {
            CopyFromSource(vm);
         }
         return vm.FieldValues.GetValue(_localCopyField);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         vm.FieldValues.SetValue(_localCopyField, value);
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _localCopyField = context.DynamicFields.DefineField<TValue>(
            DynamicFieldGroups.LocalCopyGroup
         );
      }
   }
}
