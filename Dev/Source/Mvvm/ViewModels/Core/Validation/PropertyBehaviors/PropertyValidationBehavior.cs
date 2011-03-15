namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   internal sealed class PropertyValidationBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IValidationStateProviderBehavior,
      IRevalidationBehavior {

      private static readonly FieldDefinitionGroup ValidationErrorGroup = new FieldDefinitionGroup();

      private FieldDefinition<ValidationResult> _validationStateField;
      private IVMPropertyDescriptor _property;

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         _validationStateField = context
            .Fields
            .DefineField<ValidationResult>(ValidationErrorGroup);

         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         if (Validate(context).IsValid) {
            this.SetValueNext(context, value);
         }
      }

      public ValidationResult GetValidationState(IBehaviorContext context) {
         ValidationResult state;

         if (context.FieldValues.TryGetValue(_validationStateField, out state)) {
            return state;
         } else {
            return ValidationResult.Valid;
         }
      }

      public ValidationResult GetDescendantsValidationState(IBehaviorContext context) {
         return this.GetDescendantsValidationStateNext(context);
      }

      public void Revalidate(IBehaviorContext context) {
         throw new NotImplementedException();
      }

      public void Revalidate(IBehaviorContext context, ValidationContext validationContext, ValidationMode mode) {
         switch (mode) {
            case ValidationMode.CommitValidValues:
               object displayValue = _property.Behaviors.GetDisplayValueNext(context); // TODO: Is this clean?
               _property.Behaviors.SetDisplayValueNext(context, displayValue);   // TODO: Is this clean?
               break;
            case ValidationMode.DiscardInvalidValues:
               Validate(context, validationContext);
               break;
            default:
               throw new NotSupportedException();
         }

         this.RevalidateNext(context, validationContext, mode);
      }

      internal ValidationResult Validate(IBehaviorContext context, ValidationContext validationContext) {
         Contract.Assert(_property != null, "Behavior was not properly initialized.");

         List<ValidationError> errors = new List<ValidationError>();

         var validationArgs = ValidationArgs.CreatePropertyValidationArgs(
            validationContext,
            validationErrors: errors,
            viewModel: context.VM,
            property: _property
         );

         context.NotifyValidating(validationArgs);


         // Remove errors of each collection item.

         // 


         var newState = ValidationResult.Join(
            errors
               .Where(x => x.Target == context.VM)
               .Select(x => new ValidationResult(x))
         );

         var oldState = GetValidationState(context);

         if (!newState.Equals(oldState)) {
            if (newState.IsValid) {
               context.FieldValues.ClearField(_validationStateField);
            } else {
               context.FieldValues.SetValue(_validationStateField, newState);
            }

            var args = new ChangeArgs(
               ChangeType.ValidationStateChanged,
               changedVM: context.VM,
               changedProperty: _property
            );

            context.NotifyChange(args);
         }

         return newState;
      }

      private ValidationResult Validate(IBehaviorContext context) {
         Contract.Ensures(Contract.Result<ValidationResult>() != null);

         ValidationContext.BeginValidation();
         var result = Validate(context, ValidationContext.Current);
         ValidationContext.CompleteValidation(ValidationMode.CommitValidValues);

         return result;
      }
   }
}
