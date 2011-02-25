namespace Inspiring.Mvvm.ViewModels.Core {

   public interface ICommandCanExecuteBehavior : IBehavior {
      bool CanExecute(IBehaviorContext context, object parameter);
   }
}
