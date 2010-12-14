using System.Diagnostics.Contracts;
using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PropertyValidationBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>, 
      IValidationStateProviderBehavior,
      IRevalidationBehavior {

      private static readonly FieldDefinitionGroup ValidationErrorGroup = new FieldDefinitionGroup();

      private IVMProperty _property;
      private FieldDefinition<ValidationState> _validationStateField;

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         _validationStateField = context
            .Fields
            .DefineField<ValidationState>(ValidationErrorGroup);

         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context, ValueStage stage) {
         IValueAccessorBehavior<TValue> next;
         if (TryGetBehavior(out next)) {
            return next.GetValue(context, stage);
         }
         throw new NotImplementedException();
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
