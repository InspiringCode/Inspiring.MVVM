namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public abstract class ValidationArgs {
      private readonly List<ValidationError> _errors = new List<ValidationError>();

      public void AddError(ValidationError error) {
         Contract.Requires<ArgumentNullException>(error != null);
         _errors.Add(error);
      }

      internal IList<ValidationError> GetErrors() {
         Contract.Ensures(Contract.Result<IList<ValidationError>>() != null);
         return _errors;
      }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(GetErrors() != null);
      }
   }
}
