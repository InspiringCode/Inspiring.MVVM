﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   public enum ChangeType {
      PropertyChanged,
      ValidationStateChanged
   }

   /// <summary>
   ///   Holds information about a change event (property changed, validation state
   ///   changed) that occured for a VM.
   /// </summary>
   public sealed class ChangeArgs {
      internal ChangeArgs(ChangeType changeType, IViewModel changedVM) {
         Contract.Requires(changeType == ChangeType.ValidationStateChanged);
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

      //public override bool Equals(object obj) {

      //}

      //public override int GetHashCode() {
      //   return HashCodeService.CalculateHashCode(
      //      this,
      //      ChangeType,
      //      ChangedVM,
      //      ChangedProperty
      //   );
      //}

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(ChangedVM != null);
         Contract.Invariant(
            (ChangeType == ChangeType.PropertyChanged && ChangedProperty != null) ||
            (ChangeType == ChangeType.ValidationStateChanged && ChangedProperty == null)
         );
      }
   }
}