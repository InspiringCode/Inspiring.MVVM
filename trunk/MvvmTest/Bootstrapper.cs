﻿namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class Bootstrapper {
      [AssemblyInitialize()]
      public static void Initialize(TestContext context) {
         //Contract.ContractFailed += (sender, e) => {
         //   e.SetHandled();
         //   e.SetUnwind();
         //   Assert.Fail(e.Message);
         //};

         ServiceLocator.SetServiceLocator(new ReflectionServiceLocator());
      }

      private class ReflectionServiceLocator : IServiceLocator {
         public TService GetInstance<TService>() {
            return Activator.CreateInstance<TService>();
         }

         public object TryGetInstance(Type serviceType) {
            return Activator.CreateInstance(serviceType);
         }
      }
   }
}
