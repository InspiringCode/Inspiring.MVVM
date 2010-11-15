namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IHandlePropertyChangedBehavior {
      void HandlePropertyChanged(IBehaviorContext vm);
   }
}
