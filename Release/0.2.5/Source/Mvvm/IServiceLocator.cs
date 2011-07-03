using System;
namespace Inspiring.Mvvm {
   public interface IServiceLocator {
      /// <summary>
      ///   Get an instance of the given <typeparamref name="TService"/>.
      /// </summary>
      TService GetInstance<TService>();

      /// <summary>
      ///   Gets an instance of the given <paramref name="serviceType"/> and 
      ///   returns null if no such service is registered.
      /// </summary>
      object TryGetInstance(Type serviceType);
   }
}
