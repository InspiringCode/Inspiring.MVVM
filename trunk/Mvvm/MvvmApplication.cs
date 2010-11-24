namespace Inspiring.Mvvm {
   using System.Windows;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Views;

   public abstract class MvvmApplication : Application {
      protected abstract void SetupContainer();

      protected abstract IServiceLocator GetServiceLocator();

      protected abstract void RegisterTypeIfMissing<TFrom, TTo>(bool registerAsSingleton) where TTo : TFrom;

      protected abstract Window CreateShellWindow(IWindowService windowService);

      /// <summary>
      ///   A hook method you can override to show a custom login dialog. Return true
      ///   to continue the application startup, false to shutdown the application.
      /// </summary>
      protected virtual bool Login(IWindowService windowService) {
         return true;
      }

      protected override void OnStartup(StartupEventArgs e) {
         base.OnStartup(e);
         SetupContainer();

         ServiceLocator.SetServiceLocator(GetServiceLocator());
         AddDefaultRegistrations();

         bool loginSuccessful;

         try {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            IWindowService windowService = ServiceLocator.Current.GetInstance<IWindowService>();
            loginSuccessful = Login(windowService);

            if (loginSuccessful) {
               Window shellWindow = CreateShellWindow(windowService);
               MainWindow = shellWindow;
               shellWindow.Show();
            }
         } finally {
            ShutdownMode = ShutdownMode.OnLastWindowClose;
         }

         if (!loginSuccessful) {
            Shutdown();
         }
      }

      private void AddDefaultRegistrations() {
         RegisterTypeIfMissing<IWindowService, WindowService>(registerAsSingleton: true);
         RegisterTypeIfMissing<IDialogService, WindowService>(registerAsSingleton: true);
      }
   }
}
