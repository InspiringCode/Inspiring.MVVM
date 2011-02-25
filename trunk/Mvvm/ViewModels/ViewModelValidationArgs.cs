namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   [Obsolete]
   public sealed class _ViewModelValidationArgs {
      internal _ViewModelValidationArgs(
         IViewModel validationTarget,
         IViewModel changedVM,
         IVMProperty changedProperty
      ) {
         Contract.Requires(validationTarget != null);
         Contract.Requires(changedVM != null);

         ValidationTarget = validationTarget;
         ChangedVM = changedVM;
         ChangedProperty = changedProperty;

         Errors = new List<string>();
      }

      public IViewModel ValidationTarget { get; private set; }
      public IViewModel ChangedVM { get; private set; }
      public IVMProperty ChangedProperty { get; private set; }

      internal List<string> Errors { get; private set; }

      public void AddError(string errorMessage) {
         Errors.Add(errorMessage);
      }
   }
}
