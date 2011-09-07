namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public sealed class PropertyValidatorBuilder<TOwnerVM, TTargetVM, TValue>
      where TOwnerVM : IViewModel
      where TTargetVM : IViewModel {

      private readonly ValidatorBuilderOperation _operation;

      internal PropertyValidatorBuilder(ValidatorBuilderOperation operation) {
         _operation = operation;
      }

      /// <summary>
      ///   Defines a custom validator that is executed every time the selected
      ///   property is about to change.
      /// </summary>
      /// <remarks>
      ///   The validator is also executed when a revalidation is performed, or
      ///   the VM is added to/removed from a collection.
      /// </remarks>
      public void Custom(Action<PropertyValidationArgs<TOwnerVM, TTargetVM, TValue>> validationAction) {
         Custom(DelegateValidator.For(validationAction));
      }

      /// <summary>
      ///   Defines a custom validator that is executed every time the selected
      ///   property is about to change.
      /// </summary>
      /// <remarks>
      ///   The validator is also executed when a revalidation is performed, or
      ///   the VM is added to/removed from a collection.
      /// </remarks>
      public void Custom(IValidator validator) {
         var val = new ConditionalValidator(
            new ValidationStepCondition(ValidationStep.Value),
            validator
         );

         _operation.BuildActions.Push(() => {
            _operation.ActionArgs.Push(val);
         });
      }
   }
}
