namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   Holds all infos necessary for a <see cref="Validator"/> to validate a
   ///   property or VM. It also holds the validation state (such as the 
   ///   validation errors that occur).
   /// </summary>
   /// <remarks>
   ///   <para>The <see cref="ValidationArgs"/> are initially created by the <see
   ///      cref="PropertyValidationBehavior"/> or the <see 
   ///      cref="ViewModelValidationBehavior"/> which both react to certain 
   ///      changes.</para>
   ///   <para>The <see cref="ViewModelValidationBehavior"/> of each VM in the 
   ///      hierarchy creates a new <see cref="ValidationArgs"/> object passing
   ///      in the path to the VM that has changed. For this instance the <see 
   ///      cref="OwnerVM"/> and the <see cref="TargetVM"/> are both set the VM
   ///      of the behavior. The behavor then calls all its validators that are 
   ///      defined for the given <see cref="TargetVM"/> passing in the <see 
   ///      cref="ValidationArgs"/> object.</para>
   ///   <para>After that, the <see cref="ValidationArgs"/> objects is passed 
   ///      to the parent of the VM. The parent calls <see cref="PrependTargetPath"/> 
   ///      to set the <see cref="OwnerVM"/> to itself. The <see 
   ///      cref="ViewModelValidationBehavior"/> then calls all its validators 
   ///      that it has defined for the <see cref="TargetVM"/> of the new 
   ///      <see cref="ValidationArgs"/>. The <see cref="ValidationArgs"/> are
   ///      then passed to the grand parent and the process repeats.</para>
   ///    <para>Note that each VM in the hierarchy reacts to changes and each
   ///      ancestor of a changing VM invokes the "bubbling" process described
   ///      above.</para>
   ///    <para>The same process applies to property validations.</para>
   /// </remarks>
   public class ValidationArgs {
      private readonly ValidationType _validationType;
      private readonly ValidationContext _validationContext;
      private readonly ValidationState _validationState;
      private readonly InstancePath _targetPath;
      private readonly IVMPropertyDescriptor _targetProperty;
      private readonly InstancePath _changedPath;
      private readonly IVMPropertyDescriptor _changedProperty;

      private ValidationArgs(
         ValidationType validationType,
         ValidationContext validationContext,
         ValidationState validationState,
         InstancePath changedPath,
         IVMPropertyDescriptor changedProperty,
         IVMPropertyDescriptor targetProperty,
         InstancePath targetPath
      ) {
         Contract.Requires(validationState != null);
         Contract.Requires(validationContext != null);
         Contract.Requires(targetPath != null);
         Contract.Requires(changedPath != null);
         Contract.Requires(!changedPath.IsEmpty);

         _validationType = validationType;
         _validationContext = validationContext;
         _validationState = validationState;
         _targetPath = targetPath;
         _targetProperty = targetProperty;
         _changedPath = changedPath;
         _changedProperty = changedProperty;
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
      ///   Gets the VM that has changed (view model validation) or is about to
      ///   change (property validation).
      /// </summary>
      public IViewModel ChangedVM {
         get {
            Contract.Ensures(Contract.Result<IViewModel>() != null);
            return ChangedPath.LeafVM;
         }
      }

      /// <summary>
      ///   Gets the property that should be validated. This property is only valid 
      ///   for property validations.
      /// </summary>
      public IVMPropertyDescriptor TargetProperty {
         get { return _targetProperty; }
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
      public IVMPropertyDescriptor ChangedProperty {
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
      ///   Holds VMs that should be revalidated after the current validation
      ///   process is complete.
      /// </summary>
      public RevalidationQueue RevalidationQueue {
         get {
            Contract.Ensures(Contract.Result<RevalidationQueue>() != null);
            return _validationContext.RevalidationQueue;
         }
      }

      /// <summary>
      ///   Gets what kind of value is being validated.
      /// </summary>
      internal ValidationType ValidationType {
         get {
            return _validationType;
         }
      }

      /// <summary>
      ///   Creates a <see cref="ValidationArgs"/> object for a view model 
      ///   validation that was NOT caused by a property change (it may have 
      ///   been caused by a validation state change or a collection change).
      /// </summary>
      /// <remarks>
      ///   The <see cref="OwnerVM"/> and the <see cref="TargetVM"/> are set 
      ///   to the first VM of the <paramref name="changedPath"/>.
      /// </remarks>
      /// <param name="changedPath">
      ///   The path to the VM that has changed and is causing the validation.
      /// </param>
      public static ValidationArgs CreateViewModelValidationArgs(
         ValidationContext validationContext,
         ValidationState validationState,
         InstancePath changedPath
      ) {
         Contract.Requires(validationContext != null);
         Contract.Requires(validationState != null);
         Contract.Requires(changedPath != null);
         Contract.Requires(!changedPath.IsEmpty);

         Contract.Ensures(Contract.Result<ValidationArgs>().OwnerVM == changedPath.RootVM);
         Contract.Ensures(Contract.Result<ValidationArgs>().TargetVM == changedPath.RootVM);
         Contract.Ensures(Contract.Result<ValidationArgs>().ChangedVM == changedPath.LeafVM);
         Contract.Ensures(Contract.Result<ValidationArgs>().TargetProperty == null);
         Contract.Ensures(Contract.Result<ValidationArgs>().ChangedProperty == null);

         return new ValidationArgs(
            ValidationType.ViewModel,
            validationContext,
            validationState,
            changedPath,
            changedProperty: null,
            targetProperty: null,
            targetPath: new InstancePath(changedPath.RootVM)
         );
      }

      /// <summary>
      ///   Creates a <see cref="ValidationArgs"/> object for a view model 
      ///   validation that was caused by a property change.
      /// </summary>
      /// <remarks>
      ///   The <see cref="OwnerVM"/> and the <see cref="TargetVM"/> are set 
      ///   to the first VM of the <paramref name="changedPath"/>.
      /// </remarks>
      /// <param name="changedPath">
      ///   The path to the VM that has changed and is causing the validation.
      /// </param>
      public static ValidationArgs CreateViewModelValidationArgs(
         ValidationContext validationContext,
         ValidationState validationState,
         InstancePath changedPath,
         IVMPropertyDescriptor changedProperty
      ) {
         Contract.Requires(validationContext != null);
         Contract.Requires(validationState != null);
         Contract.Requires(changedPath != null);
         Contract.Requires(!changedPath.IsEmpty);

         Contract.Ensures(Contract.Result<ValidationArgs>().OwnerVM == changedPath.RootVM);
         Contract.Ensures(Contract.Result<ValidationArgs>().TargetVM == changedPath.RootVM);
         Contract.Ensures(Contract.Result<ValidationArgs>().ChangedVM == changedPath.LeafVM);
         Contract.Ensures(Contract.Result<ValidationArgs>().TargetProperty == null);
         Contract.Ensures(Contract.Result<ValidationArgs>().ChangedProperty == changedProperty);

         return new ValidationArgs(
            ValidationType.ViewModel,
            validationContext,
            validationState,
            changedPath,
            changedProperty,
            targetProperty: null,
            targetPath: new InstancePath(changedPath.RootVM)
         );
      }

      /// <summary>
      ///   Creates a <see cref="ValidationArgs"/> object for a property 
      ///   validation.
      /// </summary>
      /// <remarks>
      ///   <para>The <see cref="OwnerVM"/>, <see cref="TargetVM"/> and <see 
      ///      cref="ChangedVM"/> are all set to the passed in <paramref 
      ///      name="viewModel"/>.</para>
      ///   <para>The <see cref="TargetProperty"/> and <see cref="ChangedProperty"/>
      ///      are both set to the passed in <paramref name="property"/> because
      ///      a property validation can only be initiated by and for the VM 
      ///      property itself.</para>
      /// </remarks>
      /// <param name="viewModel">
      ///   The VM whose property is about to change.
      /// </param>
      /// <param name="property">
      ///   The VM property that is about to change and that should be validated.
      /// </param>
      public static ValidationArgs CreatePropertyValidationArgs(
         ValidationContext validationContext,
         ValidationState validationState,
         IViewModel viewModel,
         IVMPropertyDescriptor property
      ) {
         Contract.Requires(validationContext != null);
         Contract.Requires(validationState != null);
         Contract.Requires(viewModel != null);
         Contract.Requires(property != null);

         Contract.Ensures(Contract.Result<ValidationArgs>().OwnerVM == viewModel);
         Contract.Ensures(Contract.Result<ValidationArgs>().TargetVM == viewModel);
         Contract.Ensures(Contract.Result<ValidationArgs>().ChangedVM == viewModel);
         Contract.Ensures(Contract.Result<ValidationArgs>().TargetProperty == property);
         Contract.Ensures(Contract.Result<ValidationArgs>().ChangedProperty == property);

         var viewModelPath = new InstancePath(viewModel);

         return new ValidationArgs(
            ValidationType.PropertyValue,
            validationContext,
            validationState,
            changedPath: viewModelPath,
            changedProperty: property,
            targetProperty: property,
            targetPath: viewModelPath
         );
      }

      /// <summary>
      ///   Creates a <see cref="ValidationArgs"/> object for a dispaly value
      ///   validation.
      /// </summary>
      /// <remarks>
      ///   See <see cref="CreatePropertyValidationArgs"/>.
      /// </remarks>
      /// <param name="viewModel">
      ///   The VM whose property is about to change.
      /// </param>
      /// <param name="property">
      ///   The VM property that is about to change and that should be validated.
      /// </param>
      public static ValidationArgs CreateDisplayValueValidationArgs(
         ValidationContext validationContext,
         ValidationState validationState,
         IViewModel viewModel,
         IVMPropertyDescriptor property
      ) {
         Contract.Requires(validationState != null);
         Contract.Requires(viewModel != null);
         Contract.Requires(property != null);

         Contract.Ensures(Contract.Result<ValidationArgs>().OwnerVM == viewModel);
         Contract.Ensures(Contract.Result<ValidationArgs>().TargetVM == viewModel);
         Contract.Ensures(Contract.Result<ValidationArgs>().ChangedVM == viewModel);
         Contract.Ensures(Contract.Result<ValidationArgs>().TargetProperty == property);
         Contract.Ensures(Contract.Result<ValidationArgs>().ChangedProperty == property);

         var viewModelPath = new InstancePath(viewModel);

         return new ValidationArgs(
            ValidationType.PropertyDisplayValue,
            validationContext,
            validationState,
            changedPath: viewModelPath,
            changedProperty: property,
            targetProperty: property,
            targetPath: viewModelPath
         );
      }

      /// <summary>
      ///   Returns a new <see cref="ValidationArgs"/> object whose <see 
      ///   cref="TargetPath"/> is prepended with the passed in <see 
      ///   cref="IViewModel"/>. The validation state is set to the same
      ///   instance (it is shared).
      /// </summary>
      internal ValidationArgs PrependTargetPath(IViewModel with) {
         Contract.Requires(with != null);

         InstancePath extendedTargetPath = TargetPath.PrependVM(with);

         return new ValidationArgs(
            _validationType,
            _validationContext,
            _validationState,
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
}
