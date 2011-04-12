namespace Inspiring.Mvvm.ViewModels.Core {

   internal abstract class CacheBehavior<TValue> :
      InitializableBehavior,
      IBehaviorInitializationBehavior {

      private FieldDefinition<TValue> _valueCacheField;

      public virtual void Initialize(BehaviorInitializationContext context) {
         _valueCacheField = context.Fields.DefineField<TValue>(GetFieldGroup());

         this.InitializeNext(context);
         SetInitialized();
      }

      protected void CopyFromSource(IBehaviorContext context) {
         RequireInitialized();
         TValue sourceValue = this.GetValueNext<TValue>(context);
         SetCache(context, sourceValue);
      }

      protected void CopyToSource(IBehaviorContext context) {
         RequireInitialized();

         if (HasCachedValue(context)) {
            this.SetValueNext(context, GetCachedValue(context));
         }
      }

      protected bool HasCachedValue(IBehaviorContext context) {
         RequireInitialized();
         return context.FieldValues.HasValue(_valueCacheField);
      }

      protected TValue GetCachedValue(IBehaviorContext context) {
         RequireInitialized();
         return context.FieldValues.GetValue(_valueCacheField);
      }

      protected void SetCache(IBehaviorContext context, TValue value) {
         RequireInitialized();
         context.FieldValues.SetValue(_valueCacheField, value);
      }

      protected void ClearCache(IBehaviorContext context) {
         RequireInitialized();
         context.FieldValues.ClearField(_valueCacheField);
      }

      protected abstract FieldDefinitionGroup GetFieldGroup();
   }
}
