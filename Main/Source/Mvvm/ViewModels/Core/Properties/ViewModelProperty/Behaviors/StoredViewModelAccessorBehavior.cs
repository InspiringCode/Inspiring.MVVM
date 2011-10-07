namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class StoredViewModelAccessorBehavior<TValue> :
      StoredValueAccessorBehavior<TValue>,
      IRefreshBehavior
      where TValue : IViewModel {

      public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
         TValue childVM = GetValue(context);

         if (childVM != null) {
            childVM.Kernel.RefreshWithoutValidation(executeRefreshDependencies);
         }

         this.RefreshNext(context, executeRefreshDependencies);
      }
   }
}
