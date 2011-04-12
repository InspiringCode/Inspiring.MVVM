namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IHandlePropertyChangedBehavior : IBehavior{
      void HandlePropertyChanged(IBehaviorContext vm);
   }
}
