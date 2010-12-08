namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.ViewModels.Core.Builder;

   public sealed class PropertyValidatorBuilder<TVM, TValue> :
      ConfigurationProvider
      where TVM : IViewModel {

      internal PropertyValidatorBuilder(VMDescriptorConfiguration configuration)
         : base(configuration) {
      }

      /// <summary>
      ///   Defines a custom validator that is executed every time the selected
      ///   property is about to change.
      /// </summary>
      /// <remarks>
      ///   The validator is also executed when a revalidation is performed, or
      ///   the VM is added to/removed from a collection.
      /// </remarks>
      /// <param name="validator">
      ///   An action that performs the validation. The first argument is the VM
      ///   whose property should be validated and the second argument is the new
      ///   value of the property.
      /// </param>
      public void Custom(Action<TVM, TValue, ValidationArgs> validator) {
         throw new NotImplementedException();
      }
   }
}
