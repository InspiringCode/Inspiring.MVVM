namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class CanExecuteCheckerBehavior :
      Behavior,
      ICommandExecuteBehavior {

      public void Execute(IBehaviorContext context, object parameter) {
         if (this.CanExecuteNext(context, parameter)) {
            this.ExecuteNext(context, parameter);
         }
      }
   }
}