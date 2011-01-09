namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   /// <summary>
   ///   Implements a fluent interface to configure property behaviors.
   /// </summary>
   public interface IVMBehaviorBuilder<TVM, TDescriptor> {
      /// <summary>
      ///   Configures the <see cref="BehaviorChainConfiguration"/> of a single
      ///   property.
      /// </summary>
      /// <param name="propertySelector">
      ///   Selects the property which should be configured.
      /// </param>
      ISinglePropertyBehaviorBuilder<TVM, TValue> For<TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      );
   }
}
