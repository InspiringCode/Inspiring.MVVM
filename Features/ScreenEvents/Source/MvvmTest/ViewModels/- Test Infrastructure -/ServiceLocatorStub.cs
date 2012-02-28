namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;

   public sealed class ServiceLocatorStub : ReflectionServiceLocator {
      private Dictionary<Type, Func<object>> _instances = new Dictionary<Type, Func<object>>();

      public override TService GetInstance<TService>() {
         if (_instances.ContainsKey(typeof(TService))) {
            return (TService)_instances[typeof(TService)]();
         } else {
            return base.GetInstance<TService>();
         }
      }

      public override object TryGetInstance(Type serviceType) {
         if (_instances.ContainsKey(serviceType)) {
            return _instances[serviceType]();
         } else {
            return base.TryGetInstance(serviceType);
         }
      }

      public ServiceLocatorStub Register<TInterface>(TInterface singletonImplementation) {
         _instances[typeof(TInterface)] = () => singletonImplementation;
         return this;
      }

      public ServiceLocatorStub Register<TInterface>(
         Func<TInterface> factory
      ) where TInterface : class {
         _instances[typeof(TInterface)] = factory;
         return this;
      }
   }
}
