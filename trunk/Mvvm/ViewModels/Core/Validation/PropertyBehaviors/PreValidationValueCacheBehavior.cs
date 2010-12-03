using System.Diagnostics.Contracts;
namespace Inspiring.Mvvm.ViewModels.Core.Validation.PropertyBehaviors {

   internal sealed class PreValidationValueCacheBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IRevalidationBehavior,
      IHandlePropertyChangedBehavior {

      private static readonly FieldDefinitionGroup ValueCacheGroup = new FieldDefinitionGroup();
      private FieldDefinition<TValue> _valueCacheField;

      public void Initialize(BehaviorInitializationContext context) {
         _valueCacheField = context.Fields.DefineField<TValue>(ValueCacheGroup);
      }

      public TValue GetValue(IBehaviorContext context, ValueStage stage) {
         RequireInitialized();
         return this.CallNext(x => x.GetValue(context, stage));
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();

         this.CallNext(x => x.SetValue(context, value));

         IValidationStatePropertyBehavior stateBehavior;
         if (TryGetBehavior(out stateBehavior)) {
            ValidationState state = stateBehavior.GetValidationState(context);
            if (!state.IsValid) {
               SetCache(context, value);
            } else {
               ClearCache(context);
            }
         }
      }

      public void Revalidate(IBehaviorContext context, ValidationMode mode) {
         RequireInitialized();
         if (mode == ValidationMode.DiscardInvalidValues) {
            ClearCache(context);
         }
      }

      public void HandlePropertyChanged(IBehaviorContext context) {
         RequireInitialized();
         ClearCache(context);
      }

      private void RequireInitialized() {
         Contract.Assert(
            _valueCacheField != null,
            "Behavior not initialized."
         );
      }

      private void ClearCache(IBehaviorContext context) {
         context.FieldValues.ClearField(_valueCacheField);
      }

      private void SetCache(IBehaviorContext context, TValue value) {
         context.FieldValues.SetValue(_valueCacheField, value);
      }
   }
}
