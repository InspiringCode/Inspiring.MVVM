namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   internal sealed class ViewModelValidationBehavior :
      ViewModelBehavior,
      IBehaviorInitializationBehavior {

      private FieldDefinition<ValidationState> _validationStateField;

      private List<ValidatorDefinition> _validators = new List<ValidatorDefinition>();

      public void AddValidator(VMPropertyPath targetPath, IVMProperty targetProperty, Validator validator) {
         Contract.Requires<ArgumentNullException>(targetPath != null);
         Contract.Requires<ArgumentNullException>(validator != null);

         _validators.Add(
            new ValidatorDefinition(targetPath, targetProperty, validator)
         );
      }

      public void Initialize(InitializationContext initializationContext) {
         _validationStateField = initializationContext
            .Fields
            .DefineField<ValidationState>(ViewModel.GeneralFieldGroup);
      }

      public ValidationState GetValidationState(IBehaviorContext_ context) {
         ValidationState state;

         if (context.FieldValues.TryGetValue(_validationStateField, out state)) {
            return state;
         } else {
            return ValidationState.Valid;
         }
      }

      internal void Validate(IBehaviorContext_ context) {
         Validate(
            context,
            new ValidationContext(),
            changedPath: new InstancePath(context.VM),
            changedProperty: null
         );
      }

      private void Validate(
         IBehaviorContext_ context,
         ValidationContext validationContext,
         InstancePath changedPath,
         IVMProperty changedProperty = null
      ) {
         ValidationState newState = new ValidationState();

         var validationArgs = new _ValidationArgs(
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
         IBehaviorContext_ context,
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

      protected internal override void OnValidating(IBehaviorContext_ context, _ValidationArgs args) {
         base.OnValidating(context, args);
         _validators.ForEach(val => val.Validate(args));
      }

      private class ValidatorDefinition {
         public ValidatorDefinition(VMPropertyPath targetPath, IVMProperty targetProperty, Validator validator) {
            TargetPath = targetPath;
            Validator = validator;
            TargetProperty = targetProperty;
         }
         public VMPropertyPath TargetPath { get; private set; }
         public Validator Validator { get; private set; }
         public IVMProperty TargetProperty { get; private set; }

         public void Validate(_ValidationArgs args) {
            bool isSameValidatorType = args.TargetProperty == TargetProperty;
            bool validatorPathMatches = args.TargetPath.Matches(TargetPath);

            if (isSameValidatorType && validatorPathMatches) {
               Validator.Validate(args);
            }
         }
      }
   }
}
