namespace Inspiring.Mvvm.Views {
   using System.Windows;

   public interface IWindowService {
      Window CreateWindow(Window owner, string title, bool modal);

      void ShowWindow(Window window, bool modal);
   }
}
