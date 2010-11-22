using System;
namespace Inspiring.Mvvm.ViewModels.Core {
   internal class CacheValueBehavior<TValue> : Behavior, IPropertyAccessorBehavior<TValue> {
      FieldDefinition<TValue> _localCopyField;

      protected void CopyFromSource(IBehaviorContext vm) {
         IPropertyAccessorBehavior<TValue> accessBehavior = GetNextBehavior<IPropertyAccessorBehavior<TValue>>();
         TValue sourceValue = accessBehavior.GetValue(vm, ValueStage.PostValidation);
         vm.FieldValues.SetValue(_localCopyField, sourceValue);
      }

      protected void CopyToSource(IBehaviorContext vm) {
         IPropertyAccessorBehavior<TValue> accessBehavior = GetNextBehavior<IPropertyAccessorBehavior<TValue>>();
         TValue localValue = vm.FieldValues.GetValueOrDefault(_localCopyField);
         accessBehavior.SetValue(vm, localValue);
      }

      public TValue GetValue(IBehaviorContext vm, ValueStage stage) {
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

      public void UpdateFromSource(IBehaviorContext context) {
         TValue oldValue = GetValue(context, ValueStage.PostValidation);
         CopyFromSource(context);
         TValue newValue = GetValue(context, ValueStage.PostValidation);

         if (!Object.Equals(oldValue, newValue)) {
            var args = new ChangeArgs(ChangeType.PropertyChanged, context.VM);
            context.NotifyChange(args);
         }
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
