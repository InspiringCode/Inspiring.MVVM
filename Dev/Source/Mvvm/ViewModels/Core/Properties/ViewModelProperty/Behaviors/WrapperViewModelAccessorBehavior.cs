namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class WrapperViewModelAccessorBehavior<TValue, TSource> :
      CachedAccessorBehavior<TValue>,
      IRefreshBehavior
      where TValue : IViewModel, IHasSourceObject<TSource> {

      public override void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();

         UpdateSource(context, value.Source);
         base.SetValue(context, value);
      }

      public void Refresh(IBehaviorContext context) {
         RequireInitialized();

         TValue child = GetValue(context);
         SetSourceOnViewModel(context, child);
         this.RefreshNext(context);
      }

      protected override TValue ProvideValue(IBehaviorContext context) {
         TValue child = CreateViewModel(context);
         SetSourceOnViewModel(context, child);
         return child;
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
