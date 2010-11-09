namespace Inspiring.Mvvm.ViewModels.Core.Validation {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core.Common;

   public sealed class ViewModelValidationArgs : ValidationArgs {
      public ViewModelValidationArgs(
         IViewModel validationTarget,
         InstancePath changedVM,
         VMPropertyBase changedProperty = null
      ) {
         Contract.Requires(validationTarget != null);
         Contract.Requires(changedVM != null);

         ValidationTarget = validationTarget;
         ChangedVM = changedVM;
         ChangedProperty = changedProperty;
      }

      public ViewModelValidationArgs(IViewModel validationTarget)
         : this(validationTarget, new InstancePath(validationTarget)) {

         Contract.Requires(validationTarget != null);
      }

      public IViewModel ValidationTarget { get; private set; }

      public InstancePath ChangedVM { get; private set; }

      public VMPropertyBase ChangedProperty { get; private set; }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(ValidationTarget != null);
         Contract.Invariant(ChangedVM != null);
      }
   }
}
