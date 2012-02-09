namespace Inspiring.Mvvm.Views {
   using System;
   using System.ComponentModel;
   using System.Windows;
   using Inspiring.Mvvm.Screens;

   // TODO: Make me less visible
   public class WindowCloseHandler {
      private readonly IScreenBase _screen;
      private readonly ScreenService _screenService = new ScreenService();
      private Window _window;

      public WindowCloseHandler(IScreenBase screen) {
         _screen = screen;
      }

      public virtual void AttachTo(Window window) {
         _window = window;
         _window.Closing += HandleClosing;
         _window.Closed += HandleClosed;
      }

      public virtual void Detach() {
         _window.Closing -= HandleClosing;
         _window.Closed -= HandleClosed;
      }

      protected virtual bool OnClosing(Window window) {
         return _screen.RequestClose();
      }

      protected virtual void OnClosed(Window window) {
         _screenService.DeactivateAndCloseScreen(_screen);
      }

      private void HandleClosing(object sender, CancelEventArgs e) {
         e.Cancel = !OnClosing((Window)sender);
      }

      private void HandleClosed(object sender, EventArgs e) {
         OnClosed((Window)sender);
      }
   }
}
