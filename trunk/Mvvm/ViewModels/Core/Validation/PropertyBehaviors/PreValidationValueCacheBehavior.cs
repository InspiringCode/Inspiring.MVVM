namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PreValidationValueCacheBehavior<TValue> :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IRevalidationBehavior,
      IHandlePropertyChangedBehavior {

      private static readonly FieldDefinitionGroup ValueCacheGroup = new FieldDefinitionGroup();
      private FieldDefinition<TValue> _valueCacheField;

      public void Initialize(BehaviorInitializationContext context) {
         _valueCacheField = context.Fields.DefineField<TValue>(ValueCacheGroup);
         this.InitializeNext(context);
         SetInitialized();
      }

      public TValue GetValue(IBehaviorContext context, ValueStage stage) {
         RequireInitialized();

         if (stage == ValueStage.PreValidation && HasCachedValue(context)) {
            return GetCachedValue(context);
         }

         return this.GetValueNext<TValue>(context, ValueStage.PostValidation);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();

         IValidationStateProviderBehavior stateBehavior;
         bool hasStateBehavior = TryGetBehavior(out stateBehavior);

         if (!hasStateBehavior) {
            this.SetValueNext(context, value);
            return;
         }

         // Always cache the value, so that GetValue returns the new value when
         // called in during the validation for example.
         SetCache(context, value);

         this.SetValueNext(context, value);

         bool successfullySetValue = stateBehavior
            .GetValidationState(context)
            .IsValid;

         if (successfullySetValue) {
            ClearCache(context);
         }
      }

      public void Revalidate(IBehaviorContext context, ValidationMode mode) {
         RequireInitialized();
         if (mode == ValidationMode.DiscardInvalidValues) {
            ClearCache(context); // TODO: Is this correct?
         }

         this.RevalidateNext(context, mode);
      }

      public void HandlePropertyChanged(IBehaviorContext context) {
         RequireInitialized();
         ClearCache(context);
      }

      private void ClearCache(IBehaviorContext context) {
         context.FieldValues.ClearField(_valueCacheField);
      }

      private void SetCache(IBehaviorContext context, TValue value) {
         context.FieldValues.SetValue(_valueCacheField, value);
      }

      public TValue GetCachedValue(IBehaviorContext context) {
         return context.FieldValues.GetValue(_valueCacheField);
      }

      private bool HasCachedValue(IBehaviorContext context) {
         return context.FieldValues.HasValue(_valueCacheField);
      }
   }
}
