namespace Inspiring.Mvvm.Views {
   using System.Windows;
   using Inspiring.Mvvm.Screens;

   // TODO: Make me less visible
   public class WindowCloseHandler {
      private IScreenBase _screen;

      public WindowCloseHandler(IScreenBase screen) {
         _screen = screen;
      }
      public virtual void AttachTo(Window window) {
         window.Closing += (sender, e) => {
            e.Cancel = !OnClosing((Window)sender);
         };

         window.Closed += (sender, e) => {
            OnClosed((Window)sender);
         };
      }

      protected virtual bool OnClosing(Window window) {
         return _screen.RequestClose();
      }

      protected virtual void OnClosed(Window window) {
         _screen.Deactivate();
         _screen.Close();
      }
   }
}
