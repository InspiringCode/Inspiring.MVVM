namespace Inspiring.Mvvm.Views {
   using System.Windows;
   using System.Windows.Media;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;

   public class WindowService : IWindowService {
      public static readonly Event<InitializeWindowEventArgs> InitializeWindowEvent = new Event<InitializeWindowEventArgs>();

      public virtual Window CreateWindow(Window owner, string title, bool modal) {
         Window window = new Window();

         if (title != null) {
            window.Title = title;
         }

         if (owner != null) {
            window.Owner = owner;
         }

         if (modal) {
            window.ShowInTaskbar = false;
         }

         // Needed for sharp text rendering.
         TextOptions.SetTextFormattingMode(window, TextFormattingMode.Display);
         TextOptions.SetTextRenderingMode(window, TextRenderingMode.Aliased);

         return window;
      }

      public virtual void ShowWindow(Window window, bool modal) {
         if (modal) {
            window.ShowDialog();
         } else {
            window.Show();
         }
      }
   }

   public sealed class InitializeWindowEventArgs : ScreenEventArgs {
      public InitializeWindowEventArgs(IScreenBase target, Window window)
         : base(target) {
         Window = window;
      }

      public Window Window { get; private set; }
   }
}