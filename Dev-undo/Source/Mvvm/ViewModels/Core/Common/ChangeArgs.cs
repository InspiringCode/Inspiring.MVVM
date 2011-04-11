namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
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

         ChangedPath = Path
            .Empty
            .Append(changedVM);
      }

      internal ChangeArgs(
         ChangeType changeType,
         IViewModel changedVM,
         IVMPropertyDescriptor changedProperty
      ) {
         Contract.Requires(
            changeType == ChangeType.PropertyChanged ||
            changeType == ChangeType.ValidationStateChanged
         );
         Contract.Requires(changedVM != null);
         Contract.Requires(changedProperty != null);

         ChangeType = changeType;
         ChangedVM = changedVM;
         ChangedProperty = changedProperty;

         ChangedPath = Path
            .Empty
            .Append(changedVM)
            .Append(changedProperty);
      }


      internal ChangeArgs(
         ChangeType changeType,
         IVMCollection changedCollection,
         IEnumerable<IViewModel> oldItems = null,
         IEnumerable<IViewModel> newItems = null
      ) {
         ChangeType = changeType;

         ChangedPath = Path
            .Empty
            .Append(changedCollection.Owner)
            .Append(changedCollection);

         OldItems = oldItems ?? Enumerable.Empty<IViewModel>();
         NewItems = newItems ?? Enumerable.Empty<IViewModel>();
      }

      private ChangeArgs(
         ChangeType changeType,
         IViewModel changedVM,
         IVMPropertyDescriptor changedProperty,
         Path changedPath
      ) {
         ChangeType = changeType;
         ChangedVM = changedVM;
         ChangedProperty = changedProperty;

         ChangedPath = changedPath;
      }

      public ChangeType ChangeType { get; private set; }

      public IViewModel ChangedVM { get; private set; }

      public IVMPropertyDescriptor ChangedProperty { get; private set; }

      public Path ChangedPath { get; private set; }

      public IEnumerable<IViewModel> OldItems { get; private set; }

      public IEnumerable<IViewModel> NewItems { get; private set; }

      internal ChangeArgs PrependViewModel(IViewModel viewModel) {
         return new ChangeArgs(
            ChangeType,
            ChangedVM,
            ChangedProperty,
            ChangedPath.Prepend(viewModel)
         ) { 
            NewItems = NewItems,
            OldItems = OldItems         
         };
      }

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
         //Contract.Invariant(ChangedVM != null);
         //Contract.Invariant(
         //   ChangeType == ChangeType.PropertyChanged ? ChangedProperty != null : true
         //);
      }
   }
}
