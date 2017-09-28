namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
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
      private ChangeArgs(
         ChangeType changeType,
         ValueStage stage,
         IVMPropertyDescriptor changedProperty,
         Path changedPath,
         IChangeReason reason
      ) {
         ChangeType = changeType;
         Stage = stage;
         ChangedProperty = changedProperty;

         ChangedPath = changedPath;
         Reason = reason;
      }

      private ChangeArgs(
         ChangeType changeType,
         ValueStage stage,
         Path changedPath,
         IEnumerable<IViewModel> oldItems = null,
         IEnumerable<IViewModel> newItems = null,
         IChangeReason reason = null
      ) {
         ChangeType = changeType;
         Stage = stage;
         ChangedPath = changedPath;
         OldItems = oldItems ?? Enumerable.Empty<IViewModel>();
         NewItems = newItems ?? Enumerable.Empty<IViewModel>();
         Reason = reason;
      }

      internal static ChangeArgs PropertyChanged(IVMPropertyDescriptor property, ValueStage stage, IChangeReason reason = null) {
         Check.NotNull(property, nameof(property));

         return new ChangeArgs(
            ChangeType.PropertyChanged,
            stage,
            Path.Empty.Append(property),
            reason: reason
         ) { ChangedProperty = property };// TODO: Remove
      }

      internal static ChangeArgs ViewModelPropertyChanged(
         IVMPropertyDescriptor property,
         ValueStage stage,
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
            stage,
            Path.Empty.Append(property),
            oldItems: oldItems,
            newItems: newItems,
            reason: reason
         ) { ChangedProperty = property };// TODO: Remove
      }

      internal static ChangeArgs CollectionPopulated(
         IVMCollection collection,
         IEnumerable<IViewModel> oldItems,
         IChangeReason reason = null
      ) {
         Check.NotNull(collection, nameof(collection));

         var newItems = (IEnumerable<IViewModel>)collection;

         return new ChangeArgs(
            ChangeType.CollectionPopulated,
            ValueStage.ValidatedValue,
            Path.Empty.Append(collection),
            newItems: newItems,
            oldItems: oldItems,
            reason: reason
         );
      }

      internal static ChangeArgs ItemsAdded(
         IVMCollection collection,
         IEnumerable<IViewModel> newItems,
         IChangeReason reason = null
      ) {
         Check.NotNull(collection, nameof(collection));
         Check.NotNull(newItems, nameof(newItems));
         Check.NotEmpty(newItems, nameof(newItems));

         return new ChangeArgs(
            ChangeType.AddedToCollection,
            ValueStage.ValidatedValue,
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
         Check.NotNull(collection, nameof(collection));
         Check.NotNull(oldItems, nameof(oldItems));
         Check.NotEmpty(oldItems, nameof(oldItems));

         return new ChangeArgs(
            ChangeType.RemovedFromCollection,
            ValueStage.ValidatedValue,
            Path.Empty.Append(collection),
            oldItems: oldItems,
            reason: reason
         );
      }

      internal static ChangeArgs ValidationResultChanged(IChangeReason reason = null) {
         return new ChangeArgs(
            ChangeType.ValidationResultChanged,
            ValueStage.ValidatedValue,
            Path.Empty,
            reason: reason
         );
      }

      internal static ChangeArgs ValidationResultChanged(IVMPropertyDescriptor property, ValueStage stage) {
         Check.NotNull(property, nameof(property));

         return new ChangeArgs(
            ChangeType.ValidationResultChanged,
            stage,
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

      public ValueStage Stage { get; private set; }

      internal ChangeArgs PrependViewModel(IViewModel viewModel) {
         return new ChangeArgs(
            ChangeType,
            Stage,
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
   }
}
