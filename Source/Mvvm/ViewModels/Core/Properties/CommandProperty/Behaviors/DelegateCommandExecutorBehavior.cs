namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   
   internal sealed class DelegateCommandExecutorBehavior<TSourceObject> :
      Behavior,
      ICommandCanExecuteBehavior,
      ICommandExecuteBehavior {

      private readonly Action<TSourceObject> _executeAction;
      private readonly Func<TSourceObject, bool> _canExecutePredicate;

      public DelegateCommandExecutorBehavior(
         Action<TSourceObject> executeAction,
         Func<TSourceObject, bool> canExecutePredicate = null
      ) {
         Check.NotNull(executeAction, nameof(executeAction));

         _executeAction = executeAction;
         _canExecutePredicate = canExecutePredicate;
      }

      public bool CanExecute(IBehaviorContext context, object parameter) {
         bool canExecute = true;

         if (_canExecutePredicate != null) {
            canExecute = _canExecutePredicate(GetSourceObject(context));
         }

         return canExecute && this.CanExecuteNext(context, parameter);
      }

      public void Execute(IBehaviorContext context, object parameter) {
         _executeAction(GetSourceObject(context));
         this.ExecuteNext(context, parameter);
      }

      private TSourceObject GetSourceObject(IBehaviorContext context) {
         return this.GetValueNext<TSourceObject>(context);
      }
   }
}
