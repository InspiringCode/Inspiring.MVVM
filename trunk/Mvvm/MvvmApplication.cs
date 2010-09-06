namespace Inspiring.Mvvm {
   using System.Windows;
   using Inspiring.Mvvm.Views;

   public abstract class MvvmApplication : Application {
      protected abstract void SetupContainer();

      protected abstract IServiceLocator GetServiceLocator();

      protected abstract void RegisterTypeIfMissing<TFrom, TTo>(bool registerAsSingleton) where TTo : TFrom;

      protected abstract Window CreateShellWindow(IWindowService windowService);

      protected override void OnStartup(StartupEventArgs e) {
         base.OnStartup(e);
         SetupContainer();
         ServiceLocator.SetServiceLocator(GetServiceLocator());
         AddDefaultRegistrations();
         IWindowService windowService = ServiceLocator.Current.GetInstance<IWindowService>();
         Window shellWindow = CreateShellWindow(windowService);
         MainWindow = shellWindow;
         shellWindow.Show();
      }

      private void AddDefaultRegistrations() {
         RegisterTypeIfMissing<IWindowService, WindowService>(registerAsSingleton: true);
      }
   }
}
