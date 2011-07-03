namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class DelegatingCommandBehavior<TSourceObject> :
      Behavior,
      ICommandCanExecuteBehavior,
      ICommandExecuteBehavior {

      private readonly Action<TSourceObject> _executeAction;
      private readonly Func<TSourceObject, bool> _canExecutePredicate;

      public DelegatingCommandBehavior(
         Action<TSourceObject> executeAction,
         Func<TSourceObject, bool> canExecutePredicate = null
      ) {
         Contract.Requires(executeAction != null);

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
