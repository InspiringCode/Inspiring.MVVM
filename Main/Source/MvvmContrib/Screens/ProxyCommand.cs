namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Windows.Input;

   /// <summary>
   ///   A proxy to a real ICommand.
   /// </summary>
   public class CommandProxy : ICommand {
      private ICommand _actual;
      private EventHandler _strongReferenceToHandlerDelegate;

      internal CommandProxy() {
         _strongReferenceToHandlerDelegate = new EventHandler(OnCanExecuteChanged);
      }

      public bool HasCommand { get { return _actual != null; } }

      public event EventHandler CanExecuteChanged;

      public bool CanExecute(object parameter) {
         return _actual != null ?
            _actual.CanExecute(parameter) :
            false;
      }

      public void Execute(object parameter) {
         if (_actual == null) {
            throw new InvalidOperationException(
               ExceptionTexts.ExecuteCalledWithoutActualCommand
            );
         }

         _actual.Execute(parameter);
      }

      internal void SetActualCommand(ICommand actual) {
         if (_actual != null) {
            _actual.CanExecuteChanged -= _strongReferenceToHandlerDelegate;
         }

         if (actual != null) {
            actual.CanExecuteChanged += _strongReferenceToHandlerDelegate;
         }

         _actual = actual;
         OnCanExecuteChanged(this, EventArgs.Empty);
      }

      private void OnCanExecuteChanged(object sender, EventArgs e) {
         EventHandler handler = CanExecuteChanged;
         if (handler != null) {
            handler(sender, e);
         }
      }
   }
}