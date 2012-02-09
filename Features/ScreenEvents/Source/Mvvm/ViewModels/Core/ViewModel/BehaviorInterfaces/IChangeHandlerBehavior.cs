namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IChangeHandlerBehavior : IBehavior {
      void HandleChange(IBehaviorContext context, ChangeArgs args);
   }
}
