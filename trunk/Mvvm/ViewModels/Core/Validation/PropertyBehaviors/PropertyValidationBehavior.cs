using System;
using System.Diagnostics.Contracts;
using System.Collections;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PropertyValidationBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IValidationStateProviderBehavior,
      IRevalidationBehavior {

      private static readonly FieldDefinitionGroup ValidationErrorGroup = new FieldDefinitionGroup();

      private FieldDefinition<ValidationState> _validationStateField;
      private IVMProperty _property;

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         _validationStateField = context
            .Fields
            .DefineField<ValidationState>(ValidationErrorGroup);

         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context, ValueStage stage) {
         return this.GetValueNext<TValue>(context, stage);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         if (Validate(context).IsValid) {
            this.SetValueNext(context, value);
         }
      }

      public ValidationState GetValidationState(IBehaviorContext context) {
         ValidationState state;

         if (context.FieldValues.TryGetValue(_validationStateField, out state)) {
            return state;
         } else {
            return ValidationState.Valid;
         }
      }

      public void Revalidate(IBehaviorContext context, ValidationMode mode) {
         switch (mode) {
            case ValidationMode.CommitValidValues:
               object displayValue = _property.GetValue(context, ValueStage.PreConversion);
               _property.SetValue(context, displayValue);
               break;
            case ValidationMode.DiscardInvalidValues:
               Validate(context);
               break;
            default:
               throw new NotSupportedException();
         }

         this.RevalidateNext(context, mode);
      }

      internal ValidationState Validate(IBehaviorContext context, ValidationContext validationContext) {
         Contract.Assert(_property != null, "Behavior was not properly initialized.");

         var newState = new ValidationState();

         var validationArgs = ValidationArgs.CreatePropertyValidationArgs(
            validationState: newState,
            viewModel: context.VM,
            property: _property
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
               changedVM: context.VM,
               changedProperty: _property
            );

            context.NotifyChange(args);
         }

         return newState;
      }

      internal ValidationState Validate(IBehaviorContext context) {
         Contract.Ensures(Contract.Result<ValidationState>() != null);

         ValidationContext validationContext = new ValidationContext();
         return Validate(context, validationContext);
      }
   }
}
