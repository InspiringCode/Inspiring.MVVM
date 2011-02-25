namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Windows.Input;

   public class ViewModelCommand : ICommand {
      /// <param name="owner">
      ///   The view model instance to which this command instance belongs. The
      ///   VM must be known by the command because a command always related to
      ///   a certain VM (the execute action/can execute result is different for
      ///   every VM).
      /// </param>
      /// <param name="behaviorChain">
      ///   The behavior chain the command delegates to.
      /// </param>
      public ViewModelCommand(
         IViewModel owner,
         BehaviorChain behaviorChain
      ) {
         Owner = owner;
         BehaviorChain = behaviorChain;
      }

      public event EventHandler CanExecuteChanged {
         add { CommandManager.RequerySuggested += value; }
         remove { CommandManager.RequerySuggested -= value; }
      }

      protected IViewModel Owner { get; private set; }

      protected BehaviorChain BehaviorChain { get; private set; }

      public virtual bool CanExecute(object parameter) {
         return BehaviorChain.CanExecuteNext(Owner.GetContext(), parameter);
      }

      public virtual void Execute(object parameter) {
         BehaviorChain.ExecuteNext(Owner.GetContext(), parameter);
      }
   }
}
