namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   // TODO: Make me internal please!
   public sealed class ViewModelValidationBehavior :
      ViewModelBehavior,
      IBehaviorInitializationBehavior,
      IValidationStateProviderBehavior {

      private FieldDefinition<ValidationState> _validationStateField;

      private List<ValidatorDefinition> _validators = new List<ValidatorDefinition>();

      /// <summary>
      ///   Adds a new <see cref="Validator"/> to the view model behavior.
      /// </summary>
      /// <param name="validator">
      ///   The <see cref="Validator"/> that is executed when the <paramref 
      ///   name="targetProperty"/> defined on the VM specified by <paramref 
      ///   name="targetPath"/> changes.
      /// </param>
      /// <param name="validatorType">
      ///   Specifies what validation process should trigger the validator.
      /// </param>
      /// <param name="targetPath">
      ///   The path to the descendant VM that should be validated. Pass <see 
      ///   cref="VMPropertyPath.Empty"/> if the current VM (or a property of it)
      ///   should be validated.
      /// </param>
      /// <param name="targetProperty">
      ///   The property that should be validated. Pass null if you want to add
      ///   a view model validation.
      /// </param>
      internal void AddValidator(
         Validator validator,
         ValidationType validatorType,
         VMPropertyPath targetPath,
         PropertySelector targetProperty
      ) {
         Contract.Requires<ArgumentNullException>(targetPath != null);
         Contract.Requires<ArgumentNullException>(validator != null);
         Contract.Requires<ArgumentException>(
            validatorType == ValidationType.ViewModel ?
               targetProperty == null :
               targetProperty != null
         );

         _validators.Add(
            new ValidatorDefinition(validator, validatorType, targetPath, targetProperty)
         );
      }

      public void Initialize(BehaviorInitializationContext context) {
         _validationStateField = context
            .Fields
            .DefineField<ValidationState>(ViewModel.GeneralFieldGroup);

         this.InitializeNext(context);
      }

      public ValidationState GetValidationState(IBehaviorContext context) {
         ValidationState state;

         if (context.FieldValues.TryGetValue(_validationStateField, out state)) {
            return state;
         } else {
            return ValidationState.Valid;
         }
      }

      internal void Validate(IBehaviorContext context, ValidationContext validationContext) {
         Validate(
            context,
            validationContext,
            changedPath: new InstancePath(context.VM),
            changedProperty: null
         );
      }

      private void Validate(
         IBehaviorContext context,
         ValidationContext validationContext,
         InstancePath changedPath,
         IVMPropertyDescriptor changedProperty = null
      ) {
         ValidationState newState = new ValidationState();

         var validationArgs = ValidationArgs.CreateViewModelValidationArgs(
            validationContext,
            validationState: newState,
            changedPath: changedPath,
            changedProperty: changedProperty
         );

         context.NotifyValidating(validationArgs);

         var oldState = GetValidationState(context);

         if (!newState.Equals(oldState)) {
            if (newState.IsValid) {
               context.FieldValues.ClearField(_validationStateField);
            } else {
               context.FieldValues.SetValue(_validationStateField, newState);
            }

            var args = new ChangeArgs(
               ChangeType.ValidationStateChanged,
               changedVM: context.VM
            );

            context.NotifyChange(args);
         }
      }

      protected internal override void OnChanged(
         IBehaviorContext context,
         ChangeArgs args,
         InstancePath changedPath
      ) {
         IVMCollection ownerCollection = args.ChangedVM.Kernel.OwnerCollection;

         if (ownerCollection == null || !ownerCollection.IsPopulating) {
            ValidationContext.BeginValidation();

            Validate(
               context,
               ValidationContext.Current,
               changedPath: changedPath,
               changedProperty: args.ChangedProperty
            );

            ValidationContext.CompleteValidation(ValidationMode.CommitValidValues);
         }

         base.OnChanged(context, args, changedPath);
      }

      protected internal override void OnValidating(IBehaviorContext context, ValidationArgs args) {
         base.OnValidating(context, args);
         _validators.ForEach(val => val.Validate(args));
      }

      private class ValidatorDefinition {
         public ValidatorDefinition(
            Validator validator,
            ValidationType validatorType,
            VMPropertyPath targetPath,
            PropertySelector targetProperty
         ) {
            Validator = validator;
            ValidatorType = validatorType;
            TargetPath = targetPath;
            TargetProperty = targetProperty;
         }

         public Validator Validator { get; private set; }
         public ValidationType ValidatorType { get; private set; }
         public VMPropertyPath TargetPath { get; private set; }
         public PropertySelector TargetProperty { get; private set; }

         public void Validate(ValidationArgs args) {
            if (args.ValidationType != ValidatorType) {
               return;
            }

            if (!args.TargetPath.Matches(TargetPath)) {
               return;
            }

            if (TargetProperty != null) {
               IVMPropertyDescriptor targetProperty = TargetProperty.GetProperty(args.TargetVM.Descriptor);

               if (args.TargetProperty != targetProperty) {
                  return;
               }
            }

            Validator.Validate(args);
         }
      }

      ValidationState IValidationStateProviderBehavior.GetDescendantsValidationState(IBehaviorContext context) {
         throw new NotImplementedException(); // TODO: Improve interface please (split in two)!
      }
   }
}
