namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public class _ValidationArgs {
      private readonly InstancePath _targetPath;
      private readonly IVMProperty _targetProperty;
      private readonly InstancePath _changedPath;
      private readonly IVMProperty _changedProperty;
      private readonly ValidationState _validationState;

      /// <summary>
      ///   Create a new view model validations args instance. The <see cref="OwnerVM"/>
      ///   and the <see cref="TargetVM"/> are set to the first VM of the <paramref 
      ///   name="changedPath"/>.
      /// </summary>
      public _ValidationArgs(
         ValidationState validationState,
         InstancePath changedPath
      )
         : this(
            validationState,
            changedPath,
            changedProperty: null,
            targetProperty: null,
            targetPath: new InstancePath(changedPath.RootVM)
         ) {
      }

      /// <summary>
      ///   Creates a new view model validation args instance. The <see cref="OwnerVM"/>
      ///   and the <see cref="TargetVM"/> are set to the first VM of the <paramref 
      ///   name="changedPath"/>.
      /// </summary>
      public _ValidationArgs(
         ValidationState validationState,
         InstancePath changedPath,
         IVMProperty changedProperty
      )
         : this(
            validationState,
            changedPath,
            changedProperty,
            targetProperty: null,
            targetPath: new InstancePath(changedPath.RootVM)
         ) {

         Contract.Requires(changedProperty != null);
         Contract.Requires(!changedPath.IsEmpty);
      }

      /// <summary>
      ///   <para>Creates a new property validations args instance. The <see 
      ///      cref="OwnerVM"/> and the <see cref="TargetVM"/> are set to the first 
      ///      VM of the <paramref name="changedPath"/>.</para>
      ///   <para>You have to pass the same property for <paramref name="changedProperty"/>
      ///      and the <paramref name="targetProperty"/> because a property validation
      ///      can only be triggered by a property and for that property.</para>
      /// </summary>
      public _ValidationArgs(
         ValidationState validationState,
         InstancePath changedPath,
         IVMProperty changedProperty,
         IVMProperty targetProperty
      )
         : this(
            validationState,
            changedPath,
            changedProperty,
            targetProperty,
            targetPath: new InstancePath(changedPath.RootVM)
         ) {

         Contract.Requires(changedProperty != null);
         Contract.Requires(targetProperty != null);
         Contract.Requires(changedProperty == targetProperty);
         Contract.Requires(!changedPath.IsEmpty);
      }

      private _ValidationArgs(
         ValidationState validationState,
         InstancePath changedPath,
         IVMProperty changedProperty,
         IVMProperty targetProperty,
         InstancePath targetPath
      ) {
         Contract.Requires(validationState != null);
         Contract.Requires(targetPath != null);
         Contract.Requires(changedPath != null);
         Contract.Requires(!changedPath.IsEmpty);

         _targetPath = targetPath;
         _targetProperty = targetProperty;
         _changedPath = changedPath;
         _changedProperty = changedProperty;
         _validationState = validationState;
      }

      /// <summary>
      ///   Gets the VM whose descriptor has defined the validator. 
      /// </summary>
      public IViewModel OwnerVM {
         get {
            Contract.Ensures(Contract.Result<IViewModel>() != null);
            return TargetPath.RootVM;
         }
      }

      /// <summary>
      ///   Gets the VM that should be validated.
      /// </summary>
      public IViewModel TargetVM {
         get {
            Contract.Ensures(Contract.Result<IViewModel>() != null);
            return TargetPath.LeafVM;
         }
      }

      /// <summary>
      ///   Gets a <see cref="InstancePath"/> that contains all VMs from the 
      ///   <see cref="OwnerVM"/> to the <see cref="TargetVM"/> (including both).
      /// </summary>
      public InstancePath TargetPath {
         get {
            Contract.Ensures(Contract.Result<InstancePath>() != null);
            return _targetPath;
         }
      }

      /// <summary>
      ///   Gets the property that should be validated. This property is only valid 
      ///   for property validations.
      /// </summary>
      public IVMProperty TargetProperty {
         get { return _targetProperty; }
      }

      /// <summary>
      ///   Gets the VM whose change has triggered the validation. In case of a 
      ///   property definition it contains the VM that is about to change.
      /// </summary>
      public IViewModel ChangedVM {
         get {
            Contract.Ensures(Contract.Result<IViewModel>() != null);
            return ChangedPath.LeafVM;
         }
      }

      /// <summary>
      ///   Gets a <see cref="InstancePath"/> that contains all VMs from the 
      ///   <see cref="TargetVM"/> to the <see cref="ChangedVM"/> (including both).
      /// </summary>
      public InstancePath ChangedPath {
         get {
            Contract.Ensures(Contract.Result<InstancePath>() != null);
            return _changedPath;
         }
      }

      /// <summary>
      ///   Gets the property that has changed (if any). This property is only 
      ///   valid for view model validations.
      /// </summary>
      public IVMProperty ChangedProperty {
         get { return _changedProperty; }
      }

      /// <summary>
      ///   Holds all validation errors that occur in the current validation 
      ///   process.
      /// </summary>
      public ValidationErrorCollection Errors {
         get {
            Contract.Ensures(Contract.Result<ValidationErrorCollection>() != null);
            return _validationState.Errors;
         }
      }

      /// <summary>
      ///   Returns a new <see cref="_ValidationArgs"/> object whose <see 
      ///   cref="TargetPath"/> is prepended with the passed in <paramref 
      ///   name="with"/>.
      /// </summary>
      public _ValidationArgs PrependTargetPath(IViewModel with) {
         InstancePath extendedTargetPath = TargetPath.PrependVM(with);

         return new _ValidationArgs(
            validationState: _validationState,
            changedPath: ChangedPath,
            changedProperty: ChangedProperty,
            targetProperty: TargetProperty,
            targetPath: TargetPath.PrependVM(with)
         );
      }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(OwnerVM == TargetPath.RootVM);
         Contract.Invariant(TargetVM == TargetPath.LeafVM);
         Contract.Invariant(TargetVM == ChangedPath.RootVM);
         Contract.Invariant(ChangedVM == ChangedPath.LeafVM);
      }
   }


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
