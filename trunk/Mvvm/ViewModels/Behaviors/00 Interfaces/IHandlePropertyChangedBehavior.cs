namespace Inspiring.Mvvm.ViewModels.Behaviors {

   public interface IHandlePropertyChangedBehavior {
      void HandlePropertyChanged(IBehaviorContext vm);
   }
}
