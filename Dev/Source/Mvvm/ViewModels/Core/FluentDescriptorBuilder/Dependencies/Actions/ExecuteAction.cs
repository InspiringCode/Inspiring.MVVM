namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ExecuteAction : DependencyAction {
      private readonly Action _action;
      public ExecuteAction(Action action) {
         _action = action;
      }

      public override void Execute(ChangeArgs args) {
         throw new System.NotImplementedException();
      }
   }
}
