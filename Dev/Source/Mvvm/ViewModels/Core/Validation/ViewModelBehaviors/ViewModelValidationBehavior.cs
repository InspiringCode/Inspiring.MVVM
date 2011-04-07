namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   // TODO: Make me internal please!
   public sealed class ViewModelValidationBehavior :
      ViewModelBehavior,
      IBehaviorInitializationBehavior,
      IValidationStateProviderBehavior {

      private FieldDefinition<ValidationResult> _validationStateField;

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
            .DefineField<ValidationResult>(ViewModel.GeneralFieldGroup);

         this.InitializeNext(context);
      }

      public ValidationResult GetValidationState(IBehaviorContext context) {
         ValidationResult state;

         if (context.FieldValues.TryGetValue(_validationStateField, out state)) {
            return state;
         } else {
            return ValidationResult.Valid;
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
         var errors = new List<ValidationError>();

         var validationArgs = ValidationArgs.CreateViewModelValidationArgs(
            validationContext,
            validationErrors: errors,
            changedPath: changedPath,
            changedProperty: changedProperty
         );

         context.NotifyValidating(validationArgs);

         var newState = ValidationResult.Join(errors.Select(x => new ValidationResult(x)));

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
         IVMCollection ownerCollection = args.ChangedPath[args.ChangedPath.Length - 1].Collection;

         if (ownerCollection == null || !ownerCollection.IsPopulating) {
            ValidationContext.BeginValidation();

            bool collectionChange =
               args.ChangeType == ChangeType.AddedToCollection ||
               args.ChangeType == ChangeType.RemovedFromCollection;

            bool isInitialValidationLevel = changedPath.Length == 1;

            if (collectionChange && isInitialValidationLevel) {
               // TODO: Refactor.
               args.NewItems.ForEach(x => x.Kernel.Revalidate(
                  ValidationContext.Current,
                  ValidationScope.SelfAndLoadedDescendants,
                  ValidationMode.CommitValidValues
               ));

               args.OldItems.ForEach(x => x.Kernel.Revalidate(
                  ValidationContext.Current,
                  ValidationScope.SelfAndLoadedDescendants,
                  ValidationMode.CommitValidValues
               ));
            }
            // Bugfix mit börg am 7.4.2011
            //} else {
            //   // If an item was removed or added to an collection it is enough to
            //   // call 'Revalidate' on that item, because the blow statement would
            //   // only perform a view model validation (because 'ChangedProperty'
            //   // is always null) and 'Revalidate' performs a view model validation
            //   // too.

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

      ValidationResult IValidationStateProviderBehavior.GetDescendantsValidationState(IBehaviorContext context) {
         throw new NotImplementedException(); // TODO: Improve interface please (split in two)!
      }
   }
}
