namespace Inspiring.Mvvm.ViewModels.Core {

   internal interface IRefreshBehavior : IBehavior {
      void Refresh(IBehaviorContext context);
   }
}
