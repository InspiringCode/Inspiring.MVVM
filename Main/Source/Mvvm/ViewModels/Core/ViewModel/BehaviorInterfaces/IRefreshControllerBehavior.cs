namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IRefreshControllerBehavior : IBehavior {
      void Refresh(IBehaviorContext context);
      void Refresh(IBehaviorContext context, IVMPropertyDescriptor property);
   }
}
