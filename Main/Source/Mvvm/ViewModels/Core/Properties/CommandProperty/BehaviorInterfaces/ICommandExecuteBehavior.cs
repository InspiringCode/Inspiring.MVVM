namespace Inspiring.Mvvm.ViewModels.Core {

   public interface ICommandExecuteBehavior : IBehavior {
      void Execute(IBehaviorContext context, object parameter);
   }
}
