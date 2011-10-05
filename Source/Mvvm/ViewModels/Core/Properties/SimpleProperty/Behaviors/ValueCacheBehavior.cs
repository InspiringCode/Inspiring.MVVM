namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ValueCacheBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IIsLoadedIndicatorBehavior,
      IRefreshBehavior {

      private static readonly FieldDefinitionGroup CacheGroup = new FieldDefinitionGroup();
      private DynamicFieldAccessor<TValue> _cache;

      public void Initialize(BehaviorInitializationContext context) {
         _cache = new DynamicFieldAccessor<TValue>(context, CacheGroup);
         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         TValue cachedValue;

         if (!_cache.TryGet(context, out cachedValue)) {
            cachedValue = this.GetValueNext<TValue>(context);
            _cache.Set(context, cachedValue);
         }

         return cachedValue;
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         _cache.Clear(context);
         this.SetValueNext(context, value);
      }

      public void Refresh(IBehaviorContext context) {
         _cache.Clear(context);
         this.RefreshNext(context);
      }

      public bool IsLoaded(IBehaviorContext context) {
         return _cache.HasValue(context) && this.IsLoadedNext(context);
      }
   }
}
