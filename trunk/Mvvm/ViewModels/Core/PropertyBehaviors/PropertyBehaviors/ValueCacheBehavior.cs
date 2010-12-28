namespace Inspiring.Mvvm.ViewModels.Core {
   internal class ValueCacheBehavior<TValue> :
      CacheBehavior<TValue>,
      IValueAccessorBehavior<TValue> {

      private static readonly FieldDefinitionGroup ValueCacheGroup = new FieldDefinitionGroup();

      public TValue GetValue(IBehaviorContext context, ValueStage stage) {
         RequireInitialized();

         if (!HasCachedValue(context)) {
            CopyFromSource(context);
         }

         return GetCache(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();
         SetCache(context, value);
         this.SetValueNext(context, value);
      }
      
      protected override FieldDefinitionGroup GetFieldGroup() {
         return ValueCacheGroup;
      }
   }
}
