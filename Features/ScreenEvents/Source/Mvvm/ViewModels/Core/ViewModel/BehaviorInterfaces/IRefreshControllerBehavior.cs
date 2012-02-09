namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IRefreshControllerBehavior : IBehavior {
      void Refresh(IBehaviorContext context, bool executeRefreshDependencies);
      void Refresh(IBehaviorContext context, IVMPropertyDescriptor property, RefreshOptions options);
   }
}
