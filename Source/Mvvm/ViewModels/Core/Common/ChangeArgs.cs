namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   public enum ChangeType {
      PropertyChanged,
      ValidationResultChanged,
      CollectionPopulated,
      AddedToCollection,
      RemovedFromCollection
   }

   public interface IChangeReason {
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
         IVMPropertyDescriptor changedProperty,
         Path changedPath,
         IChangeReason reason
      ) {
         ChangeType = changeType;
         ChangedProperty = changedProperty;

         ChangedPath = changedPath;
         Reason = reason;
      }

      private ChangeArgs(
         ChangeType changeType,
         Path changedPath,
         IEnumerable<IViewModel> oldItems = null,
         IEnumerable<IViewModel> newItems = null,
         IChangeReason reason = null
      ) {
         ChangeType = changeType;
         ChangedPath = changedPath;
         OldItems = oldItems ?? Enumerable.Empty<IViewModel>();
         NewItems = newItems ?? Enumerable.Empty<IViewModel>();
         Reason = reason;
      }

      internal static ChangeArgs PropertyChanged(IVMPropertyDescriptor property, IChangeReason reason = null) {
         Contract.Requires(property != null);

         return new ChangeArgs(
            ChangeType.PropertyChanged,
            Path.Empty.Append(property),
            reason: reason
         ) { ChangedProperty = property };// TODO: Remove
      }

      internal static ChangeArgs ViewModelPropertyChanged(
         IVMPropertyDescriptor property,
         IViewModel oldValue,
         IViewModel newValue,
         IChangeReason reason = null
      ) {
         var oldItems = oldValue != null ?
            new[] { oldValue } :
            null;

         var newItems = newValue != null ?
            new[] { newValue } :
            null;

         return new ChangeArgs(
            ChangeType.PropertyChanged,
            Path.Empty.Append(property),
            oldItems: oldItems,
            newItems: newItems,
            reason: reason
         ) { ChangedProperty = property };// TODO: Remove
      }

      internal static ChangeArgs CollectionPopulated(IVMCollection collection, IChangeReason reason = null) {
         Contract.Requires(collection != null);

         var newItems = (IEnumerable<IViewModel>)collection;

         return new ChangeArgs(
            ChangeType.CollectionPopulated,
            Path.Empty.Append(collection),
            newItems: newItems,
            reason: reason
         );
      }

      internal static ChangeArgs ItemsAdded(
         IVMCollection collection,
         IEnumerable<IViewModel> newItems,
         IChangeReason reason = null
      ) {
         Contract.Requires(collection != null);
         Contract.Requires(newItems != null);
         Contract.Requires(newItems.Any());

         return new ChangeArgs(
            ChangeType.AddedToCollection,
            Path.Empty.Append(collection),
            newItems: newItems,
            reason: reason
         );
      }

      internal static ChangeArgs ItemsRemoved(
         IVMCollection collection,
         IEnumerable<IViewModel> oldItems,
         IChangeReason reason = null
      ) {
         Contract.Requires(collection != null);
         Contract.Requires(oldItems != null);
         Contract.Requires(oldItems.Any());

         return new ChangeArgs(
            ChangeType.RemovedFromCollection,
            Path.Empty.Append(collection),
            oldItems: oldItems,
            reason: reason
         );
      }

      internal static ChangeArgs ValidationResultChanged(IChangeReason reason = null) {
         return new ChangeArgs(
            ChangeType.ValidationResultChanged,
            Path.Empty,
            reason: reason
         );
      }

      internal static ChangeArgs ValidationResultChanged(IVMPropertyDescriptor property) {
         Contract.Requires(property != null);

         return new ChangeArgs(
            ChangeType.ValidationResultChanged,
            Path.Empty.Append(property)
         );
      }

      public ChangeType ChangeType { get; private set; }

      public IViewModel ChangedVM {
         get {
            for (int i = ChangedPath.Length - 1; i >= 0; i--) {
               var step = ChangedPath[i];
               if (step.Type == PathStepType.ViewModel) {
                  return step.ViewModel;
               }
            }
            return null;
         }
      }

      public IVMPropertyDescriptor ChangedProperty { get; private set; }

      public Path ChangedPath { get; private set; }

      public IEnumerable<IViewModel> OldItems { get; private set; }

      public IEnumerable<IViewModel> NewItems { get; private set; }

      public IChangeReason Reason { get; private set; }

      internal ChangeArgs PrependViewModel(IViewModel viewModel) {
         return new ChangeArgs(
            ChangeType,
            ChangedProperty,
            ChangedPath.Prepend(viewModel),
            Reason
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
