namespace Inspiring.Mvvm.ViewModels.Core {
   internal class ValueCacheBehavior<TValue> :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue> {

      private static readonly FieldDefinitionGroup ValueCacheGroup = new FieldDefinitionGroup();
      private DynamicFieldAccessor<TValue> _cache;

      public void Initialize(BehaviorInitializationContext context) {
         _cache = new DynamicFieldAccessor<TValue>(context, ValueCacheGroup);
         SetInitialized();

         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         RequireInitialized();

         if (!_cache.HasValue(context)) {
            var value = this.CreateValueNext<TValue>(context);
            _cache.Set(context, value);
            return value;
         }

         return _cache.Get(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();

         _cache.Set(context, value);
         this.SetValueNext(context, value);
      }
   }

   internal class ValueCacheBehaviorOld<TValue> :
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

      public new void ClearCache(IBehaviorContext context) {
         base.ClearCache(context);
      }

      protected override FieldDefinitionGroup GetFieldGroup() {
         return ValueCacheGroup;
      }
   }
}
