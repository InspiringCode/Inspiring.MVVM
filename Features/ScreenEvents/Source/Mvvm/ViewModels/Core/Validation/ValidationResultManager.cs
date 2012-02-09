namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ValidationResultManager {
      private readonly DynamicFieldAccessor<ValidationResult> _resultField;
      private readonly IVMPropertyDescriptor _property;
      private readonly ValueStage _stage;

      public ValidationResultManager(
         BehaviorInitializationContext context,
         FieldDefinitionGroup fieldGroup,
         ValueStage stage
      ) {
         _resultField = new DynamicFieldAccessor<ValidationResult>(context, fieldGroup);
         _property = context.Property;
         _stage = stage;
      }

      public void UpdateValidationResult(IBehaviorContext context, ValidationResult result) {
         var previousResult = GetValidationResult(context);
         if (!result.Equals(previousResult)) {
            SetOrClearValidationResult(context, result);

            var args = _property != null ?
               ChangeArgs.ValidationResultChanged(_property, _stage) :
               ChangeArgs.ValidationResultChanged();

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
