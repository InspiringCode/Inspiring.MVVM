namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;

   public sealed class ServiceLocatorStub : ReflectionServiceLocator {
      private Dictionary<Type, object> _instances = new Dictionary<Type, object>();

      public override TService GetInstance<TService>() {
         if (_instances.ContainsKey(typeof(TService))) {
            return (TService)_instances[typeof(TService)];
         } else {
            return base.GetInstance<TService>();
         }
      }

      public override object TryGetInstance(Type serviceType) {
         if (_instances.ContainsKey(serviceType)) {
            return _instances[serviceType];
         } else {
            return base.TryGetInstance(serviceType);
         }
      }

      public ServiceLocatorStub Register<TInterface>(TInterface implementation) {
         _instances[typeof(TInterface)] = implementation;
         return this;
      }
   }
}
