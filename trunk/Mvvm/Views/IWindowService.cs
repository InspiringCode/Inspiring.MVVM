namespace Inspiring.Mvvm.Views {
   using System.Windows;
   using Inspiring.Mvvm.Screens;

   public interface IWindowService {
      Window CreateWindow<TScreen>(
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreenBase;

      void ConfigureWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreenBase;
   }
}
