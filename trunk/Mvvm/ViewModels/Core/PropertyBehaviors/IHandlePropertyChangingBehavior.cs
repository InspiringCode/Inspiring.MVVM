namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IHandlePropertyChangingBehavior {
      void HandlePropertyChanging(IBehaviorContext vm);
   }
}
