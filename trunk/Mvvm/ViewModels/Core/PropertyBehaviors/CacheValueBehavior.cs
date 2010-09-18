namespace Inspiring.Mvvm.ViewModels.Core {
   internal class CacheValueBehavior<TValue> : Behavior, IAccessPropertyBehavior<TValue> {
      FieldDefinition<TValue> _localCopyField;

      protected void CopyFromSource(IBehaviorContext vm) {
         IAccessPropertyBehavior<TValue> accessBehavior = GetNextBehavior<IAccessPropertyBehavior<TValue>>();
         TValue sourceValue = accessBehavior.GetValue(vm);
         vm.FieldValues.SetValue(_localCopyField, sourceValue);
      }

      protected void CopyToSource(IBehaviorContext vm) {
         IAccessPropertyBehavior<TValue> accessBehavior = GetNextBehavior<IAccessPropertyBehavior<TValue>>();
         TValue localValue = vm.FieldValues.GetValueOrDefault(_localCopyField);
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

   internal sealed class RefreshableValueCahche<TValue> : CacheValueBehavior<TValue>, IManuelUpdateBehavior {
      private VMPropertyBase<TValue> _property;

      public void UpdateFromSource(IBehaviorContext vm) {
         CopyFromSource(vm);
         vm.RaisePropertyChanged(_property);
      }

      public void UpdateSource(IBehaviorContext vm) {
         CopyToSource(vm);
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _property = (VMPropertyBase<TValue>)context.Property;
      }
   }

}
