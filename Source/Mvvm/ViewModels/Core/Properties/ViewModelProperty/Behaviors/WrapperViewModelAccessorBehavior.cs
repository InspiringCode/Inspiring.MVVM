namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class WrapperViewModelAccessorBehavior<TValue, TSource> :
      CachedAccessorBehavior<TValue>,
      IRefreshBehavior
      where TValue : IViewModel, IHasSourceObject<TSource> {

      public override void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();

         var newSourceValue = value != null ?
            value.Source :
            default(TSource);

         UpdateSource(context, newSourceValue);
         base.SetValue(context, value);
      }

      public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
         RequireInitialized();

         TSource source = this.GetValueNext<TSource>(context);
         TValue childVM = GetValue(context);

         // Note: We must not call 'SetValueNext' here because a Refresh would
         // try to set the source property which may fail if it is a readonly
         // property.

         if (source != null) {
            if (childVM != null) {
               childVM.Source = source;
               childVM.Kernel.RefreshWithoutValidation(executeRefreshDependencies);
            } else {
               childVM = CreateViewModel(context);
               childVM.Source = source;
               UpdateCache(context, childVM);
            }
         } else {
            UpdateCache(context, default(TValue));
         }

         this.RefreshNext(context, executeRefreshDependencies);
      }

      protected override TValue ProvideValue(IBehaviorContext context) {
         TSource source = this.GetValueNext<TSource>(context);
         TValue childVM = default(TValue);

         if (source != null) {
            childVM = CreateViewModel(context);
            childVM.Source = source;
         }

         return childVM;
      }

      private TValue CreateViewModel(IBehaviorContext context) {
         var factory = GetNextBehavior<IValueFactoryBehavior<TValue>>();
         return factory.CreateValue(context);
      }

      private void SetSourceOnViewModel(IBehaviorContext context, TValue child) {
         TSource source = this.GetValueNext<TSource>(context);
         child.Source = source;
      }

      private void UpdateSource(IBehaviorContext context, TSource sourceValue) {
         this.SetValueNext<TSource>(context, sourceValue);
      }
   }
}
