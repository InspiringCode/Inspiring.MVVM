namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class DelegateViewModelAccessorBehavior<TValue> :
      CachedAccessorBehavior<TValue>,
      IRefreshBehavior {

      public void Refresh(IBehaviorContext context) {
         RequireInitialized();

         RefreshCache(context);
         this.RefreshNext(context);
      }

      protected override TValue ProvideValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }
   }
}
