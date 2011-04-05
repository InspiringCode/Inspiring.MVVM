namespace Inspiring.Mvvm.ViewModels.Core {

   /// <summary>
   ///   This action is the very first action on the undo stack.  When the user
   ///   requests a rollback point, the top most action on the undo stack is returned.
   ///   Its possible that a rollback point is requested before modifcations are made.
   ///   In this case this action is returned.
   /// </summary>
   internal sealed class InitialPseudoAction : IUndoableAction {

   }
}
