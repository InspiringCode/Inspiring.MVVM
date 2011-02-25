namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IIsLoadedIndicatorBehavior : IBehavior {
      bool IsLoaded(IBehaviorContext context);
   }
}
