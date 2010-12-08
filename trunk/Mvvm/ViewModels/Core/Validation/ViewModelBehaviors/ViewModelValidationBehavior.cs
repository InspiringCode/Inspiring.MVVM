﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   internal sealed class ViewModelValidationBehavior :
      ViewModelBehavior,
      IBehaviorInitializationBehavior {

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
      public void AddValidator(
         Validator validator,
         ValidationType validatorType,
         VMPropertyPath targetPath,
         IVMProperty targetProperty
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

      public void Initialize(BehaviorInitializationContext initializationContext) {
         _validationStateField = initializationContext
            .Fields
            .DefineField<ValidationState>(ViewModel.GeneralFieldGroup);
      }

      public ValidationState GetValidationState(IBehaviorContext context) {
         ValidationState state;

         if (context.FieldValues.TryGetValue(_validationStateField, out state)) {
            return state;
         } else {
            return ValidationState.Valid;
         }
      }

      internal void Validate(IBehaviorContext context) {
         Validate(
            context,
            new ValidationContext(),
            changedPath: new InstancePath(context.VM),
            changedProperty: null
         );
      }

      private void Validate(
         IBehaviorContext context,
         ValidationContext validationContext,
         InstancePath changedPath,
         IVMProperty changedProperty = null
      ) {
         ValidationState newState = new ValidationState();

         var validationArgs = ValidationArgs.CreateViewModelValidationArgs(
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
         Validate(
            context,
            new ValidationContext(),
            changedPath: changedPath,
            changedProperty: args.ChangedProperty
         );

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
            IVMProperty targetProperty
         ) {
            Validator = validator;
            ValidatorType = validatorType;
            TargetPath = targetPath;
            TargetProperty = targetProperty;
         }

         public Validator Validator { get; private set; }
         public ValidationType ValidatorType { get; private set; }
         public VMPropertyPath TargetPath { get; private set; }
         public IVMProperty TargetProperty { get; private set; }

         public void Validate(ValidationArgs args) {
            bool sameValidatorType = args.ValidationType == ValidatorType;
            bool validatorPathMatches = args.TargetPath.Matches(TargetPath);
            bool sameTargetProperty = args.TargetProperty == TargetProperty;

            if (sameValidatorType && validatorPathMatches && sameTargetProperty) {
               Validator.Validate(args);
            }
         }
      }
   }
}
