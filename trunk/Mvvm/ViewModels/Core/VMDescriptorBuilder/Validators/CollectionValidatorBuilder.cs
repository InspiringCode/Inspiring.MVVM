namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core.Builder;

   public sealed class CollectionValidatorBuilder<TItemVM> :
      ConfigurationProvider
      where TItemVM : IViewModel {

      internal CollectionValidatorBuilder(VMDescriptorConfiguration configuration)
         : base(configuration) {
      }

      /// <summary>
      ///   Defines a custom validator that is executed after an an item of the
      ///   selected collection has changed.
      /// </summary>
      /// <remarks>
      ///   The validator is also executed when a revalidation is performed, the 
      ///   VM is added to/removed from a collection or any descendant VM has
      ///   changed (either one of its properties or its validation state).
      /// </remarks>
      /// <param name="validator">
      ///   An action that performs the validation. The first argument is the
      ///   collection item that has changed, the second argument contains all
      ///   collection items.
      /// </param>
      public void Custom(Action<TItemVM, IEnumerable<TItemVM>, ValidationArgs> validator) {

      }
   }
}
