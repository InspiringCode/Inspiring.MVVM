namespace Inspiring.MvvmExample {
   using System;
   using System.Windows;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Views;
   using Inspiring.MvvmExample.Screens;
   using Inspiring.MvvmExample.Views;
   using StructureMap;

   public partial class App : MvvmApplication {
      protected override void SetupContainer() {
         ObjectFactory.Configure(c => {
            c.For<IView<ShellScreen>>().Use<ShellView>();
         });
      }

      protected override IServiceLocator GetServiceLocator() {
         return new StructureMapServiceLocator();
      }

      protected override void RegisterTypeIfMissing<TFrom, TTo>(bool registerAsSingleton) {
         if (!ObjectFactory.Container.Model.HasImplementationsFor<TFrom>()) {
            ObjectFactory.Configure(c => {
               if (registerAsSingleton) {
                  c.For<TFrom>().Singleton().Use<TTo>();
               } else {
                  c.For<TFrom>().Use<TTo>();
               }
            });
         }
      }

      protected override Window CreateAndShowShellWindow(IWindowService windowService) {
         // TODO: ScreenFactory.For<ShellScreen>()
         Window window = windowService.CreateWindow(null, null, false);
         window.Show();
         return window;
      }

      protected override bool Login(IWindowService windowService) {
         // TODO: Create a more correct example (with screen and so)!
         new Window().ShowDialog();
         return false;
      }

      private class StructureMapServiceLocator : IServiceLocator {
         public TService GetInstance<TService>() {
            return ObjectFactory.GetInstance<TService>();
         }

         public object TryGetInstance(Type serviceType) {
            return ObjectFactory.TryGetInstance(serviceType);
         }
      }
   }
}
