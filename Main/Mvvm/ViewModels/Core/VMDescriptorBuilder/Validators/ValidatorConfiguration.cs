namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal class ValidatorConfiguration {
      private readonly ViewModelValidationBehavior _validationBehavior;
      private readonly VMPropertyPath _targetPath;
      private readonly PropertySelector _targetProperty;

      protected ValidatorConfiguration() {
         _targetPath = VMPropertyPath.Empty;
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

      protected virtual ViewModelValidationBehavior ValidationBehavior {
         get { return _validationBehavior; }
      }

      /// <summary>
      ///   Returns a new <see cref="ValidatorConfiguration"/> with its <see 
      ///   cref="TargetProperty"/> set to the passed in property.
      /// </summary>
      public virtual ValidatorConfiguration SetTargetProperty(PropertySelector selector) {
         return new ValidatorConfiguration(
            ValidationBehavior,
            _targetPath,
            selector
         );
      }

      /// <summary>
      ///   Returns a new <see cref="ValidatorConfiguration"/> whos TargetPath is
      ///   extended by the passed VM property.
      /// </summary>
      public virtual ValidatorConfiguration ExtendTargetPath(PropertySelector selector) {
         return new ValidatorConfiguration(
            ValidationBehavior,
            _targetPath.AddProperty(selector),
            _targetProperty
         );
      }

      public virtual void AddViewModelValidator(Validator validator) {
         Contract.Requires(validator != null);

         ValidationBehavior.AddValidator(
            validator,
            ValidationType.ViewModel,
            TargetPath,
            null
         );
      }

      public virtual void AddPropertyValidator(Validator validator) {
         Contract.Requires(validator != null);
         Contract.Requires(TargetProperty != null);

         ValidationBehavior.AddValidator(
            validator,
            ValidationType.PropertyValue,
            TargetPath,
            TargetProperty
         );
      }

      public virtual void AddDisplayValueValidator(Validator validator) {
         Contract.Requires(validator != null);
         Contract.Requires(TargetProperty != null);

         ValidationBehavior.AddValidator(
            validator,
            ValidationType.PropertyDisplayValue,
            TargetPath,
            TargetProperty
         );
      }
   }
}
