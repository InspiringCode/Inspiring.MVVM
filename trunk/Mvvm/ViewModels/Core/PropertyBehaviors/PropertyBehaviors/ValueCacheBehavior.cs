namespace Inspiring.Mvvm.ViewModels.Core {
   internal class ValueCacheBehavior<TValue> :
      CacheBehavior<TValue>,
      IValueAccessorBehavior<TValue> {

      private static readonly FieldDefinitionGroup ValueCacheGroup = new FieldDefinitionGroup();

      public TValue GetValue(IBehaviorContext context) {
         RequireInitialized();

         if (!HasCachedValue(context)) {
            CopyFromSource(context);
         }

         return GetCachedValue(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();
         SetCache(context, value);
         this.SetValueNext(context, value);
      }

      public new bool HasCachedValue(IBehaviorContext context) {
         return base.HasCachedValue(context);
      }

      protected override FieldDefinitionGroup GetFieldGroup() {
         return ValueCacheGroup;
      }
   }
}
