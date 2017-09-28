namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Holds all infos necessary for a <see cref="Validator"/> to validate a
   ///   property or VM. It also holds the validation state (such as the 
   ///   validation errors that occur).
   /// </summary>
   /// <remarks>
   ///   TODO: Update documentation!
   ///   <para>The <see cref="ValidationArgs"/> are initially created by the
   ///      'PropertyValidationBehavior' or the
   ///      'ViewModelValidationBehavior' which both react to certain 
   ///      changes.</para>
   ///   <para>The 'ViewModelValidationBehavior' of each VM in the 
   ///      hierarchy creates a new <see cref="ValidationArgs"/> object passing
   ///      in the path to the VM that has changed. For this instance the
   ///      'OwnerVM' and the 'TargetVM' are both set the VM
   ///      of the behavior. The behavor then calls all its validators that are 
   ///      defined for the given 'TargetVM' passing in the
   ///      'ValidationArgs' object.</para>
   ///   <para>After that, the <see cref="ValidationArgs"/> objects is passed 
   ///      to the parent of the VM. The parent calls 'PrependTargetPath'
   ///      to set the 'OwnerVM' to itself. The
   ///      'ViewModelValidationBehavior' then calls all its validators 
   ///      that it has defined for the 'TargetVM' of the new 
   ///      <see cref="ValidationArgs"/>. The <see cref="ValidationArgs"/> are
   ///      then passed to the grand parent and the process repeats.</para>
   ///    <para>Note that each VM in the hierarchy reacts to changes and each
   ///      ancestor of a changing VM invokes the "bubbling" process described
   ///      above.</para>
   ///    <para>The same process applies to property validations.</para>
   /// </remarks>

   public abstract class ValidationArgs {
      protected ValidationArgs(ValidationStep step, IValidator validator) {
         Check.NotNull(validator, nameof(validator));

         Step = step;
         Validator = validator;
         Result = ValidationResult.Valid;
      }

      internal ValidationStep Step { get; private set; }

      internal IValidator Validator { get; private set; }

      internal ValidationResult Result { get; private set; }

      protected void AddError(ValidationError error) {
         Result = ValidationResult.Join(Result, new ValidationResult(error));
      }
   }

   public abstract class ValidationArgs<TOwnerVM> : ValidationArgs
      where TOwnerVM : IViewModel {

      protected ValidationArgs(ValidationStep step, IValidator validator, TOwnerVM owner)
         : base(step, validator) {

         Check.NotNull(owner, nameof(owner));
         Owner = owner;
      }

      public TOwnerVM Owner { get; private set; }
   }
}
