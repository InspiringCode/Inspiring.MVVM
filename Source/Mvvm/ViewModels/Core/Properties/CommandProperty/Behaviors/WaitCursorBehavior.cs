namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Windows.Input;
   using System.Windows.Threading;

   internal sealed class WaitCursorBehavior :
      Behavior,
      ICommandExecuteBehavior {

      public void Execute(IBehaviorContext context, object parameter) {
         Mouse.OverrideCursor = Cursors.Wait;

         // It is a bad idea, to reset the cursor after the next handler has
         // executed, because the handler may call Window.ShowDialog() which
         // does not return unless the modal dialog is closed.
         Dispatcher.CurrentDispatcher.BeginInvoke(
            new Action(() => Mouse.OverrideCursor = null),
            DispatcherPriority.ApplicationIdle
         );

         this.ExecuteNext(context, parameter);
      }
   }
}
