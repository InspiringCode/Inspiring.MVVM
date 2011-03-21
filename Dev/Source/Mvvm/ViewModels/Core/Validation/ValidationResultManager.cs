namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ValidationResultManager {
      private DynamicFieldAccessor<ValidationResult> _resultField;
      private IVMPropertyDescriptor _property;

      public ValidationResultManager(
         BehaviorInitializationContext context,
         FieldDefinitionGroup fieldGroup
      ) {
         _resultField = new DynamicFieldAccessor<ValidationResult>(context, fieldGroup);
         _property = context.Property;
      }

      public void UpdateValidationResult(IBehaviorContext context, ValidationResult result) {
         var previousResult = GetValidationResult(context);
         if (!result.Equals(previousResult)) {
            SetOrClearValidationResult(context, result);

            var isViewModelChange = _property == null;
            var changeType = ChangeType.ValidationStateChanged;
            var changedVM = context.VM;

            var args = isViewModelChange ?
               new ChangeArgs(changeType, changedVM) :
               new ChangeArgs(changeType, changedVM, _property);

            context.NotifyChange(args);
         }
      }

      public ValidationResult GetValidationResult(IBehaviorContext context) {
         return _resultField.GetWithDefault(context, ValidationResult.Valid);
      }

      private void SetOrClearValidationResult(IBehaviorContext context, ValidationResult result) {
         if (result.IsValid) {
            _resultField.Clear(context);
         } else {
            _resultField.Set(context, result);
         }
      }
   }
}
