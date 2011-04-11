namespace Inspiring.Mvvm.Views {
   using System.Windows;
   using Inspiring.Mvvm.Screens;

   public interface IWindowService {
      Window CreateWindow<TScreen>(
         IScreenFactory<TScreen> initializeWithScreen
      ) where TScreen : IScreenBase;

      Window CreateDialogWindow<TScreen>(
         IScreenFactory<TScreen> initializeWithScreen
      ) where TScreen : IScreenBase;

      Window CreateShellWindow<TScreen>(
         IScreenFactory<TScreen> initializeWithScreen
      ) where TScreen : IScreenBase;

      void InitializeWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreenBase;

      void InitializeDialogWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreenBase;

      void ShowDialogWindow(IScreenBase screen, IScreenBase parent, string title);

      Window GetAssociatedWindow(IScreenBase screen);
   }
}
