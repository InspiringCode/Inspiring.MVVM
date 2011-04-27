﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   public sealed class CollectionValidatorBuilder<TItemDescriptor, TItemValue>
      where TItemDescriptor : VMDescriptorBase {

      private ValidatorConfiguration _configuration;

      internal CollectionValidatorBuilder(ValidatorConfiguration configuration) {
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
      public void Custom<TItemVM>(
         Action<TItemVM, IEnumerable<TItemVM>, IVMPropertyDescriptor<TItemValue>, ValidationArgs> validator
       ) where TItemVM : IViewModel {
         Contract.Requires<ArgumentNullException>(validator != null);
         _configuration.AddPropertyValidator(new DelegateValidator<TItemVM>(validator));
      }

      private sealed class DelegateValidator<TItemVM> : Validator {
         private Action<TItemVM, IEnumerable<TItemVM>, IVMPropertyDescriptor<TItemValue>, ValidationArgs> _validatorCallback;

         public DelegateValidator(Action<TItemVM, IEnumerable<TItemVM>, IVMPropertyDescriptor<TItemValue>, ValidationArgs> validatorCallback) {
            Contract.Requires(validatorCallback != null);
            _validatorCallback = validatorCallback;
         }

         public override void ValidateCore(ValidationArgs args) {
            Contract.Assert(args.TargetProperty != null);

            var item = (TItemVM)args.TargetVM;
            var items = (IEnumerable<TItemVM>)args
               .TargetVM
               .Kernel
               .OwnerCollections
               .SelectMany(x => x.Cast<TItemVM>())
               .ToArray();

            var property = (IVMPropertyDescriptor<TItemValue>)args.TargetProperty;

            _validatorCallback(item, items, property, args);
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