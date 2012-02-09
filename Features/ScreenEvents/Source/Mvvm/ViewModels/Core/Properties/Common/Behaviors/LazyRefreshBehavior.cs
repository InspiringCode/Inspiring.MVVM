namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class LazyRefreshBehavior : Behavior, IRefreshBehavior {
      public void Refresh(IBehaviorContext context, RefreshOptions options) {
         if (this.IsLoadedNext(context)) {
            this.RefreshNext(context, options);
         }
      }
   }
}
