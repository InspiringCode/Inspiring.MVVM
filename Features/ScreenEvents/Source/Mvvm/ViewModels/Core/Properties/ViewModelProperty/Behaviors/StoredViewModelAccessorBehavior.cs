namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class StoredViewModelAccessorBehavior<TValue> :
      StoredValueAccessorBehavior<TValue>,
      IRefreshBehavior
      where TValue : IViewModel {

      public void Refresh(IBehaviorContext context, RefreshOptions options) {
         TValue childVM = GetValue(context);

         if (childVM != null) {
            childVM.Kernel.RefreshWithoutValidation(options.ExecuteRefreshDependencies);
         }

         this.RefreshNext(context, options);
      }
   }
}
