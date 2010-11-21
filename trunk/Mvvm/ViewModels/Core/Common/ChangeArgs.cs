namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   public enum ChangeType {
      PropertyChanged,
      ValidationStateChanged,
      AddedToCollection,
      RemovedFromCollection
   }

   /// <summary>
   ///   Holds information about a change event (property changed, validation state
   ///   changed) that occured for a VM.
   /// </summary>
   public sealed class ChangeArgs {
      internal ChangeArgs(ChangeType changeType, IViewModel changedVM) {
         Contract.Requires(changeType != ChangeType.PropertyChanged);
         Contract.Requires(changedVM != null);

         ChangeType = changeType;
         ChangedVM = changedVM;
      }

      internal ChangeArgs(
         ChangeType changeType,
         IViewModel changedVM,
         IVMProperty changedProperty
      ) {
         Contract.Requires(changeType == ChangeType.PropertyChanged);
         Contract.Requires(changedVM != null);
         Contract.Requires(changedProperty != null);

         ChangeType = changeType;
         ChangedVM = changedVM;
         ChangedProperty = changedProperty;
      }

      public ChangeType ChangeType { get; private set; }

      public IViewModel ChangedVM { get; private set; }

      public IVMProperty ChangedProperty { get; private set; }

      public override bool Equals(object obj) {
         ChangeArgs other = obj as ChangeArgs;
         return
            other != null &&
            other.ChangeType == ChangeType &&
            other.ChangedVM == ChangedVM &&
            other.ChangedProperty == ChangedProperty;
      }

      public override int GetHashCode() {
         return HashCodeService.CalculateHashCode(
            this,
            ChangeType,
            ChangedVM,
            ChangedProperty
         );
      }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(ChangedVM != null);
         Contract.Invariant(
            (ChangeType == ChangeType.PropertyChanged && ChangedProperty != null) ||
            (ChangeType != ChangeType.PropertyChanged && ChangedProperty == null)
         );
      }
   }
}
