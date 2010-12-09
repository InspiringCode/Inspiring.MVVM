﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   public sealed class PropertyValidatorBuilder<TVM, TValue> where TVM : IViewModel {
      private ViewModelValidationBehavior _validationBehavior;

      internal PropertyValidatorBuilder(ViewModelValidationBehavior behavior) {
         Contract.Requires(behavior != null);
         _validationBehavior = behavior;
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
         _validationBehavior.AddValidator(
            new DelegateValidator(validator),
            ValidationType.PropertyValue,
            VMPropertyPath.Empty,
            null
         );
      }

      private sealed class DelegateValidator : Validator {
         private Action<TVM, TValue, ValidationArgs> _validatorCallback;

         public DelegateValidator(Action<TVM, TValue, ValidationArgs> validatorCallback) {
            Contract.Requires(validatorCallback != null);
            _validatorCallback = validatorCallback;
         }

         public override void Validate(ValidationArgs args) {
            TVM vm = (TVM)args.TargetVM;
            TValue value = (TValue)args.TargetVM.GetValue(args.TargetProperty);
            _validatorCallback(vm, value, args);
         }

         public override string ToString() {
            return String.Format(
               "{{DelegateValidator: {0}}}",
               DelegateUtils.GetFriendlyName(_validatorCallback)
            );
         }
      }
   }
}
