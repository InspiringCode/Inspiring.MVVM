namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
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
      //internal ChangeArgs(ChangeType changeType, IViewModel changedVM) {
      //   Contract.Requires(changeType == ChangeType.ValidationStateChanged);
      //   Contract.Requires(changedVM != null);

      //   ChangeType = changeType;
      //   ChangedVM = changedVM;

      //   ChangedPath = Path
      //      .Empty
      //      .Append(changedVM);
      //}

      //internal ChangeArgs(
      //   ChangeType changeType,
      //   IViewModel changedVM,
      //   IVMPropertyDescriptor changedProperty
      //) {
      //   Contract.Requires(
      //      changeType == ChangeType.PropertyChanged ||
      //      changeType == ChangeType.ValidationStateChanged
      //   );
      //   Contract.Requires(changedVM != null);
      //   Contract.Requires(changedProperty != null);

      //   ChangeType = changeType;
      //   ChangedVM = changedVM;
      //   ChangedProperty = changedProperty;

      //   ChangedPath = Path
      //      .Empty
      //      .Append(changedVM)
      //      .Append(changedProperty);
      //}

      //internal ChangeArgs(
      //   ChangeType changeType,
      //   IVMCollection changedCollection,
      //   IEnumerable<IViewModel> oldItems = null,
      //   IEnumerable<IViewModel> newItems = null
      //) {
      //   ChangeType = changeType;

      //   ChangedPath = Path
      //      .Empty
      //      .Append(changedCollection.Owner)
      //      .Append(changedCollection);

      //   OldItems = oldItems ?? Enumerable.Empty<IViewModel>();
      //   NewItems = newItems ?? Enumerable.Empty<IViewModel>();
      //}

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

      private ChangeArgs(
         ChangeType changeType,
         Path changedPath,
         IEnumerable<IViewModel> oldItems = null,
         IEnumerable<IViewModel> newItems = null
      ) {
         ChangeType = changeType;
         ChangedPath = changedPath;
         OldItems = oldItems ?? Enumerable.Empty<IViewModel>();
         NewItems = newItems ?? Enumerable.Empty<IViewModel>();
      }

      internal static ChangeArgs PropertyChanged(IVMPropertyDescriptor property) {
         Contract.Requires(property != null);

         return new ChangeArgs(
            ChangeType.PropertyChanged,
            Path.Empty.Append(property)
         ) { ChangedProperty = property };// TODO: Remove
      }

      internal static ChangeArgs ItemsAdded(IVMCollection collection, IEnumerable<IViewModel> newItems) {
         Contract.Requires(collection != null);
         Contract.Requires(newItems != null);
         Contract.Requires(newItems.Any());

         return new ChangeArgs(
            ChangeType.AddedToCollection,
            Path.Empty.Append(collection),
            newItems: newItems
         );
      }

      internal static ChangeArgs ItemsRemoved(IVMCollection collection, IEnumerable<IViewModel> oldItems) {
         Contract.Requires(collection != null);
         Contract.Requires(oldItems != null);
         Contract.Requires(oldItems.Any());

         return new ChangeArgs(
            ChangeType.RemovedFromCollection,
            Path.Empty.Append(collection),
            oldItems: oldItems
         );
      }

      internal static ChangeArgs ValidationStateChanged() {
         return new ChangeArgs(
            ChangeType.ValidationStateChanged,
            Path.Empty
         );
      }

      internal static ChangeArgs ValidationStateChanged(IVMPropertyDescriptor property) {
         Contract.Requires(property != null);

         return new ChangeArgs(
            ChangeType.ValidationStateChanged,
            Path.Empty.Append(property)
         );
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

      public override string ToString() {
         return String.Format(
            "{{ChangeArgs {0}, Path = {1}}}",
            ChangeType,
            ChangedPath
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
