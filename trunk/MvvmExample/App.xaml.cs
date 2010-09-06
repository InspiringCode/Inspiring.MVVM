﻿namespace Inspiring.MvvmExample {
   using System;
   using System.Windows;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Screens;
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

      protected override Window CreateShellWindow(IWindowService windowService) {
         return windowService.CreateWindow(ScreenFactory.For<ShellScreen>());
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
