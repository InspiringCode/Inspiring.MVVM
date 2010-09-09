namespace Inspiring.Mvvm.Views {
   using System;
   using System.ComponentModel;
   using System.Windows;
   using System.Windows.Media;
   using Inspiring.Mvvm.Screens;

   public class WindowService : IWindowService {
      public virtual Window CreateWindow<TScreen>(IScreenFactory<TScreen> screen) where TScreen : IScreen {
         Window window = CreateWindow();
         ConfigureWindow(window, screen);
         return window;
      }

      protected virtual Window CreateWindow() {
         Window window = new Window();

         // Needed for sharp text rendering.
         TextOptions.SetTextFormattingMode(window, TextFormattingMode.Display);
         TextOptions.SetTextRenderingMode(window, TextRenderingMode.Aliased);

         return window;
      }

      public virtual void ConfigureWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreen {
         TScreen s = forScreen.Create(new ScreenInitializer(parent: null));
         s.Activate();

         View.SetModel(window, s);

         window.Closing += delegate(object sender, CancelEventArgs e) {
            e.Cancel = !s.RequestClose();
         };

         window.Closed += delegate(object sender, EventArgs e) {
            s.Deactivate();
            s.Close();
         };
      }

   }
}
