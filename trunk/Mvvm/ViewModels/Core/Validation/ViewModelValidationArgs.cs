namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   public sealed class ViewModelValidationArgs : ValidationArgs {
      public ViewModelValidationArgs(
         IViewModel validationOwner,
         IViewModel validationTarget,
         IViewModel changedVM,
         IVMProperty changedProperty = null
      ) {
         Contract.Requires(validationOwner != null);
         Contract.Requires(validationTarget != null);
         Contract.Requires(changedVM != null);

         ValidationTarget = validationTarget;
         ChangedVM = changedVM;
         ChangedProperty = changedProperty;
      }

      public ViewModelValidationArgs(IViewModel validationTarget)
         : this(
            validationOwner: validationTarget,
            validationTarget: validationTarget,
            changedVM: validationTarget
         ) {

         Contract.Requires(validationTarget != null);
      }

      public IViewModel ValidationOwner { get; private set; }

      public IViewModel ValidationTarget { get; private set; }

      public IViewModel ChangedVM { get; private set; }

      public IVMProperty ChangedProperty { get; private set; }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(ValidationTarget != null);
         Contract.Invariant(ChangedVM != null);
      }
   }
}
