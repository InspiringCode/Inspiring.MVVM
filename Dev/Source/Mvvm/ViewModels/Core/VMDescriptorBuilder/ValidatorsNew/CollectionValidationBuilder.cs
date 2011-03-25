namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.ValidatorsNew {
   using System;
   using System.Collections.Generic;

   public sealed class CollectionValidatorBuilder<TItemVM> where TItemVM : IViewModel {
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
         throw new NotImplementedException();
      }
   }

   public sealed class CollectionValidatorBuilder<TItemDescriptor, TItemValue>
      where TItemDescriptor : VMDescriptorBase {

      /// <summary>
      ///   Defines a custom validator that is executed when the selected property
      ///   is about to change on any item of the selected collection.
      /// </summary>
      /// <remarks>
      ///   The validator is also executed when a revalidation is performed, or
      ///   the VM is added to/removed from a collection.
      /// </remarks>
      /// <param name="validator">
      ///   An action that performs the validation. The first argument is the
      ///   new value of the changin property, the second argument contains the
      ///   property values of all collection items.
      /// </param>
      public void Custom<TItemVM>(
         Action<TItemVM, IEnumerable<TItemVM>, IVMPropertyDescriptor<TItemValue>, ValidationArgs> validator
       ) where TItemVM : IViewModel {
         throw new NotImplementedException();
      }
   }
}
