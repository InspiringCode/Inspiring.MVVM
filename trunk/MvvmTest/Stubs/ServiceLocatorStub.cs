namespace Inspiring.MvvmTest.Stubs {
   using System;
   using Inspiring.Mvvm;

   internal sealed class ServiceLocatorStub : IServiceLocator {
      public ServiceLocatorStub(object objectToReturn = null) {
         ObjectToReturn = objectToReturn;
      }

      public object ObjectToReturn { get; set; }

      public TService GetInstance<TService>() {
         return (TService)ObjectToReturn;
      }

      public object TryGetInstance(Type serviceType) {
         return ObjectToReturn;
      }
   }
}
