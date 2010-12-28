namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PreValidationValueCacheBehavior<TValue> :
      CacheBehavior<TValue>,
      IValueAccessorBehavior<TValue>,
      IRevalidationBehavior,
      IHandlePropertyChangedBehavior {

      private static readonly FieldDefinitionGroup ValueCacheGroup = new FieldDefinitionGroup();
      private IVMProperty _property;

      public override void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         base.Initialize(context);
      }

      public TValue GetValue(IBehaviorContext context, ValueStage stage) {
         RequireInitialized();

         if (stage == ValueStage.PreValidation && HasCachedValue(context)) {
            return GetCache(context);
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

         if (mode == ValidationMode.DiscardInvalidValues && HasCachedValue(context)) {
            ClearCache(context);

            this.RevalidateNext(context, mode);

            var args = new ChangeArgs(ChangeType.PropertyChanged, context.VM, _property);
            context.NotifyChange(args);
         } else {
            this.RevalidateNext(context, mode);
         }
      }

      public void HandlePropertyChanged(IBehaviorContext context) {
         RequireInitialized();
         ClearCache(context);
      }

      protected override FieldDefinitionGroup GetFieldGroup() {
         return ValueCacheGroup;
      }
   }
}
