using System.Diagnostics.Contracts;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PropertyValidationBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      //IAccessPropertyBehavior<TValue>, // TODO: Add back interface after refactoring.
      IValidationStatePropertyBehavior {

      private static readonly FieldDefinitionGroup ValidationErrorGroup = new FieldDefinitionGroup();

      private IVMProperty _property;
      private FieldDefinition<ValidationState> _validationStateField;

      public void Initialize(InitializationContext context) {
         _property = context.Property;
         _validationStateField = context
            .Fields
            .DefineField<ValidationState>(ValidationErrorGroup);
      }

      public TValue GetValue(IPropertyBehaviorContext vm) {
         return this.CallNext(x => x.GetValue(vm));
      }

      public void SetValue(IPropertyBehaviorContext vm, TValue value) {
         if (Validate(vm).IsValid) {
            this.CallNext(x => x.SetValue(vm, value));
         }
      }

      public ValidationState GetValidationState(IPropertyBehaviorContext context) {
         ValidationState state;

         if (context.FieldValues.TryGetValue(_validationStateField, out state)) {
            return state;
         } else {
            return ValidationState.Valid;
         }
      }

      internal ValidationState Validate(IPropertyBehaviorContext context, ValidationContext validationContext) {
         Contract.Assert(_property != null, "Behavior was not properly initialized.");

         var newState = new ValidationState();
         context.NotifyPropertyValidating(_property, newState);

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

      internal ValidationState Validate(IPropertyBehaviorContext context) {
         Contract.Ensures(Contract.Result<ValidationState>() != null);

         ValidationContext validationContext = new ValidationContext();
         return Validate(context, validationContext);
      }
   }
}
