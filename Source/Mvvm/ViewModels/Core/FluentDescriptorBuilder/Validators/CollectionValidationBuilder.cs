﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public sealed class CollectionValidatorBuilder<TOwnerVM, TItemVM>
      where TOwnerVM : IViewModel
      where TItemVM : IViewModel {

      private readonly ValidatorBuilderOperation _operation;

      internal CollectionValidatorBuilder(ValidatorBuilderOperation operation) {
         _operation = operation;
      }

      /// <summary>
      ///   Defines a custom validator that is executed after an an item of the
      ///   selected collection has changed.
      /// </summary>
      /// <remarks>
      ///   The validator is also executed when a revalidation is performed, the 
      ///   VM is added to/removed from a collection or any descendant VM has
      ///   changed (a property or its validation state).
      /// </remarks>
      public void Custom(Action<CollectionValidationArgs<TOwnerVM, TItemVM>> validationAction) {
         _operation.PushValidatorBuildActions(
            DelegateValidator.For(validationAction),
            ValidationStep.ViewModel
         );
      }
   }

   public sealed class CollectionValidatorBuilder<TOwnerVM, TItemDescriptor, TValue>
      where TOwnerVM : IViewModel
      where TItemDescriptor : IVMDescriptor {

      private readonly ValidatorBuilderOperation _operation;

      internal CollectionValidatorBuilder(ValidatorBuilderOperation operation) {
         _operation = operation;
      }

      /// <summary>
      ///   Defines a custom validator that is executed when the selected property
      ///   is about to change on any item of the selected collection.
      /// </summary>
      /// <remarks>
      ///   The validator is also executed when a revalidation is performed, or
      ///   the VM is added to/removed from a collection.
      /// </remarks>
      public void Custom<TItemVM>(
         Action<CollectionValidationArgs<TOwnerVM, TItemVM, TValue>> validationAction
      ) where TItemVM : IViewModel {
         _operation.PushValidatorBuildActions(
            DelegateValidator.For(validationAction),
            ValidationStep.Value
         );
      }
   }
}
