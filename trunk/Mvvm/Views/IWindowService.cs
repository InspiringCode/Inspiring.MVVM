namespace Inspiring.Mvvm.Views {
   using System.Windows;
   using Inspiring.Mvvm.Screens;

   public interface IWindowService {
      Window CreateWindow<TScreen>(IScreenFactory<TScreen> screen) where TScreen : IScreen;
   }
}
