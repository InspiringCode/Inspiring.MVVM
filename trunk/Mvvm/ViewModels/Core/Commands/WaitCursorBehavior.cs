namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Windows.Input;

   internal sealed class WaitCursorBehavior :
      Behavior,
      ICommandExecuteBehavior {

      public void Execute(IBehaviorContext context, object parameter) {
         try {
            Mouse.OverrideCursor = Cursors.Wait;
            this.ExecuteNext(context, parameter);
         } finally {
            Mouse.OverrideCursor = null;
         }
      }
   }
}
