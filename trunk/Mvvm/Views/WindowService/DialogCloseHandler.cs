namespace Inspiring.Mvvm.Views {
   using System.Windows;
   using Inspiring.Mvvm.Screens;

   // TODO: Make me less visible
   public class DialogCloseHandler : WindowCloseHandler {
      private IScreenBase _dialog;
      private bool _closeIsUserRequested = true;

      public DialogCloseHandler(IScreenBase dialog)
         : base(dialog) {
         _dialog = dialog;
      }

      public override void AttachTo(Window window) {
         base.AttachTo(window);
         DialogLifecycle
            .GetDialogLifecycle(_dialog)
            .CloseWindow += (sender, e) => {
               _closeIsUserRequested = false;
               window.Close();
            };
      }

      protected override bool OnClosing(Window window) {
         DialogLifecycle
            .GetDialogLifecycle(_dialog)
            .WindowResult = window.DialogResult;

         return _closeIsUserRequested ?
            base.OnClosing(window) :
            true;
      }

      protected override void OnClosed(Window window) {
         DialogLifecycle
            .GetDialogLifecycle(_dialog)
            .WindowResult = window.DialogResult;

         base.OnClosed(window);
      }
   }
}
