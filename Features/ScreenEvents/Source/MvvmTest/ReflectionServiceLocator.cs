namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm;

   public class ReflectionServiceLocator : IServiceLocator {
      public virtual TService GetInstance<TService>() {
         return Activator.CreateInstance<TService>();
      }

      public virtual object TryGetInstance(Type serviceType) {
         return Activator.CreateInstance(serviceType);
      }
   }
}
