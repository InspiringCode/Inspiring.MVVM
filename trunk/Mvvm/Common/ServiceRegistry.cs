namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;

   /// <summary>
   ///   A simple dictionary of Type to instance mappings.
   /// </summary>
   public class ServiceRegistry {
      private Dictionary<Type, object> _instances;

      public TService GetService<TService>() {
         object instance = null;
         bool serivceRegistered =
            _instances != null &&
            _instances.TryGetValue(typeof(TService), out instance);

         if (serivceRegistered) {
            return (TService)instance;
         }

         throw new ArgumentException(
            ExceptionTexts.ServiceNotRegistered.FormatWith(typeof(TService).Name)
         );
      }

      public void RegisterService<TService>(TService instance) {
         if (_instances == null) {
            _instances = new Dictionary<Type, object>();
         }

         if (_instances.ContainsKey(typeof(TService))) {
            throw new ArgumentException(
               ExceptionTexts.ServiceAlreadyRegistered.FormatWith(typeof(TService).Name)
            );
         }

         _instances.Add(typeof(TService), instance);
      }
   }
}
