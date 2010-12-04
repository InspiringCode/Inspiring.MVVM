namespace Inspiring.Mvvm.ViewModels.Core {
   internal class ValueCacheBehavior<TValue> :
      InitializableBehavior,
      IValueAccessorBehavior<TValue>,
      IBehaviorInitializationBehavior {
      FieldDefinition<TValue> _valueCacheField;

      protected void CopyFromSource(IBehaviorContext vm) {
         RequireInitialized();

         IValueAccessorBehavior<TValue> accessBehavior = GetNextBehavior<IValueAccessorBehavior<TValue>>();
         TValue sourceValue = accessBehavior.GetValue(vm, ValueStage.PostValidation);
         vm.FieldValues.SetValue(_valueCacheField, sourceValue);
      }

      protected void CopyToSource(IBehaviorContext vm) {
         RequireInitialized();

         IValueAccessorBehavior<TValue> accessBehavior = GetNextBehavior<IValueAccessorBehavior<TValue>>();
         TValue localValue = vm.FieldValues.GetValueOrDefault(_valueCacheField);
         accessBehavior.SetValue(vm, localValue);
      }

      public TValue GetValue(IBehaviorContext vm, ValueStage stage = ValueStage.PreValidation) {
         RequireInitialized();

         if (!vm.FieldValues.HasValue(_valueCacheField)) {
            CopyFromSource(vm);
         }
         return vm.FieldValues.GetValue(_valueCacheField);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         RequireInitialized();
         vm.FieldValues.SetValue(_valueCacheField, value);
      }

      public virtual void Initialize(BehaviorInitializationContext context) {
         _valueCacheField = context.Fields.DefineField<TValue>(
            DynamicFieldGroups.ValueCacheGroup
         );

         SetInitialized();

         this.CallNext(x => x.Initialize(context));
      }
   }
}
