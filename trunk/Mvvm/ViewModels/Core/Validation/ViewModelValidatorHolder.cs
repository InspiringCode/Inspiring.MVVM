namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A service registered with a <see cref="VMDescriptor"/> that holds all
   ///   view model level validators.
   /// </summary>
   public sealed class ViewModelValidatorHolder {
      private List<Action<ViewModelValidationArgs>> _validators =
         new List<Action<ViewModelValidationArgs>>();

      public bool IsSealed { get; private set; }

      public void AddValidator(Action<ViewModelValidationArgs> validator) {
         Contract.Requires<ArgumentNullException>(validator != null);
         Contract.Requires<InvalidOperationException>(
            !IsSealed,
            "Validators cannot be added once they are already used by a view model. " +
            "Add all validators before instantiating the first view model that uses them."
         );

         _validators.Add(validator);
      }

      internal IEnumerable<Action<ViewModelValidationArgs>> GetValidators() {
         IsSealed = true;
         return _validators;
      }
   }
}
