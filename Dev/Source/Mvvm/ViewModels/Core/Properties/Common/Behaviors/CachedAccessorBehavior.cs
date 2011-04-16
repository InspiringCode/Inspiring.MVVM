using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   public abstract class CachedAccessorBehavior<TValue> :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IIsLoadedIndicatorBehavior {

      private static readonly FieldDefinitionGroup ValueCacheGroup = new FieldDefinitionGroup();
      private static readonly FieldDefinitionGroup IsProvidingGroup = new FieldDefinitionGroup();
      private DynamicFieldAccessor<TValue> _cache;
      private DynamicFieldAccessor<bool> _isProviding;
      private IVMPropertyDescriptor _property;

      public virtual void Initialize(BehaviorInitializationContext context) {
         _cache = new DynamicFieldAccessor<TValue>(context, ValueCacheGroup);
         _isProviding = new DynamicFieldAccessor<bool>(context, IsProvidingGroup);
         _property = context.Property;
         SetInitialized();

         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         RequireInitialized();

         if (!_cache.HasValue(context)) {
            if (_isProviding.GetWithDefault(context, false)) {
               // This check is important to avoid the following endless recursion:
               //   1. GetValue calls ProvideValue
               //   2. ProvideValue directly or indirectly calls GetValue
               //   3. GetValue calls Provide value because the cache is not set yet.
               //   4. ProvideValue calls GetValue again
               // 
               // The alternative would be to return default(TValue) but it is better
               // to avoid this situation altogehter.
               throw new InvalidOperationException(EViewModels.ValueAccessedWithinProvideValue);
            }

            _isProviding.Set(context, true);
            TValue value = ProvideValue(context);
            _cache.Set(context, value);
            _isProviding.Clear(context);

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
