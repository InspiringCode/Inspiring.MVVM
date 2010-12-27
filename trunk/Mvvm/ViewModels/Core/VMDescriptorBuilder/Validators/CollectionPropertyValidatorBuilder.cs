namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;
   using System.Collections;

   public sealed class CollectionPropertyValidatorBuilder<TItemValue> {
      private ValidatorConfiguration _configuration;

      internal CollectionPropertyValidatorBuilder(ValidatorConfiguration configuration) {
         Contract.Requires(configuration != null);
         _configuration = configuration;
      }

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
      public void Custom(Action<TItemValue, IEnumerable<TItemValue>, ValidationArgs> validator) {
         Contract.Requires<ArgumentNullException>(validator != null);
         _configuration.AddPropertyValidator(new DelegateValidator(validator));
      }

      private sealed class DelegateValidator : Validator {
         private Action<TItemValue, IEnumerable<TItemValue>, ValidationArgs> _validatorCallback;

         public DelegateValidator(Action<TItemValue, IEnumerable<TItemValue>, ValidationArgs> validatorCallback) {
            Contract.Requires(validatorCallback != null);
            _validatorCallback = validatorCallback;
         }

         public override void Validate(ValidationArgs args) {
            TItemValue value = (TItemValue)args.TargetVM.GetValue(args.TargetProperty);
            IEnumerable<TItemValue> values = args
               .TargetVM
               .Kernel.OwnerCollection
               .Cast<IViewModel>()
               .Select(x => (TItemValue)x.GetValue(args.TargetProperty));

            _validatorCallback(value, values, args);
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
