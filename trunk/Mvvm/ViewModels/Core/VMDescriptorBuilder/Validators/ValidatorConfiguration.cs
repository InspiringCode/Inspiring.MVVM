namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class ValidatorConfiguration {
      private readonly ViewModelValidationBehavior _validationBehavior;
      private readonly VMPropertyPath _targetPath;
      private readonly PropertySelector _targetProperty;

      public ValidatorConfiguration(ViewModelValidationBehavior validationBehavior)
         : this(validationBehavior, VMPropertyPath.Empty, null) {
         Contract.Requires(validationBehavior != null);
      }

      private ValidatorConfiguration(
         ViewModelValidationBehavior validationBehavior,
         VMPropertyPath targetPath,
         PropertySelector targetProperty
      ) {
         _validationBehavior = validationBehavior;
         _targetPath = targetPath;
         _targetProperty = targetProperty;
      }

      public VMPropertyPath TargetPath {
         get { return _targetPath; }
      }

      public PropertySelector TargetProperty {
         get { return _targetProperty; }
      }

      /// <summary>
      ///   Returns a new <see cref="ValidatorConfiguration"/> with its <see 
      ///   cref="TargetProperty"/> set to the passed in property.
      /// </summary>
      public ValidatorConfiguration SetTargetProperty(PropertySelector selector) {
         return new ValidatorConfiguration(_validationBehavior, _targetPath, selector);
      }

      /// <summary>
      ///   Returns a new <see cref="ValidatorConfiguration"/> whos TargetPath is
      ///   extended by the passed VM property.
      /// </summary>
      public ValidatorConfiguration ExtendTargetPath(PropertySelector selector) {
         return new ValidatorConfiguration(
            _validationBehavior,
            _targetPath.AddProperty(selector),
            _targetProperty
         );
      }

      public void AddViewModelValidator(Validator validator) {
         Contract.Requires(validator != null);

         _validationBehavior.AddValidator(
            validator,
            ValidationType.ViewModel,
            TargetPath,
            null
         );
      }

      public void AddPropertyValidator(Validator validator) {
         Contract.Requires(validator != null);
         Contract.Requires(TargetProperty != null);

         _validationBehavior.AddValidator(
            validator,
            ValidationType.ViewModel,
            TargetPath,
            TargetProperty
         );
      }

      public void AddDisplayValueValidator(Validator validator) {
         Contract.Requires(validator != null);
         Contract.Requires(TargetProperty != null);

         _validationBehavior.AddValidator(
            validator,
            ValidationType.PropertyDisplayValue,
            TargetPath,
            TargetProperty
         );
      }
   }
}
