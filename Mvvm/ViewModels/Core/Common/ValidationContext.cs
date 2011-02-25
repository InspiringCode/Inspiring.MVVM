namespace Inspiring.Mvvm.ViewModels.Core.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class ValidationContext {
      public ValidationContext(
         IViewModel targetVM,
         IVMProperty targetProperty,
         object newValue
      ) {
         Contract.Requires(targetVM != null);

         TargetVM = targetVM;
         TargetProperty = targetProperty;
         NewValue = newValue;

         Errors = new List<ValidationError>();
      }

      public IVMProperty TargetProperty { get; private set; }

      public object NewValue { get; private set; }

      public IViewModel TargetVM { get; private set; }

      internal List<ValidationError> Errors { get; private set; }

      public void AddError(ValidationError error) {
         Contract.Requires<ArgumentNullException>(error != null);
         Errors.Add(error);
      }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(TargetVM != null);
      }
   }
}
