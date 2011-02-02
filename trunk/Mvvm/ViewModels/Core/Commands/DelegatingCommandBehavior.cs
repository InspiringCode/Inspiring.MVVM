namespace Inspiring.Mvvm.ViewModels.Core.Commands {
   using System;

   internal sealed class DelegatingCommandBehavior<TSourceObject> :
      Behavior,
      ICommandCanExecuteBehavior,
      ICommandExecuteBehavior {

      public DelegatingCommandBehavior() {

      }

      public bool CanExecute(IBehaviorContext context, object parameter) {
         throw new NotImplementedException();
      }

      public void Execute(IBehaviorContext context, object parameter) {
         throw new NotImplementedException();
      }
   }
}
