namespace Inspiring.Mvvm.ViewModels.Core {

   public abstract class CachedAccessorBehavior<TValue> :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IIsLoadedIndicatorBehavior {

      private static readonly FieldDefinitionGroup ValueCacheGroup = new FieldDefinitionGroup();
      private DynamicFieldAccessor<TValue> _cache;
      private IVMPropertyDescriptor _property;

      public virtual void Initialize(BehaviorInitializationContext context) {
         _cache = new DynamicFieldAccessor<TValue>(context, ValueCacheGroup);
         _property = context.Property;
         SetInitialized();

         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         RequireInitialized();

         if (!_cache.HasValue(context)) {
            TValue value = ProvideValue(context);
            _cache.Set(context, value);

            _property
               .Behaviors
               .InitializeValueNext(context);
         }

         return _cache.Get(context);
      }

      public virtual void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();
         _cache.Set(context, value);
         this.SetValueNext(context, value);
      }

      public bool IsLoaded(IBehaviorContext context) {
         return _cache.HasValue(context) && this.IsLoadedNext(context);
      }

      protected void RefreshCache(IBehaviorContext context) {
         _cache.Clear(context);
         GetValue(context);
      }

      protected abstract TValue ProvideValue(IBehaviorContext context);
   }
}
