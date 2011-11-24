namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class UserDefinedCompensationAction : IUndoableAction {
      private readonly Action _action;

      public UserDefinedCompensationAction(Action action) {
         _action = action;
      }

      public void Undo() {
         if (_action != null) {
            _action();
         }
      }
   }
}
