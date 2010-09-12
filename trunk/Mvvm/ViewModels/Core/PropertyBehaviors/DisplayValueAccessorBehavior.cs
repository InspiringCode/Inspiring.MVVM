namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class DisplayValueAccessorBehavior<TValue> : Behavior, IAccessPropertyBehavior, IValidationBehavior {
      private FieldDefinition<string> _conversionErrorField;
      private string _propertyName;

      public DisplayValueAccessorBehavior(string propertyName) {
         _propertyName = propertyName;
      }

      public object GetValue(IBehaviorContext vm) {
         IAccessPropertyBehavior<TValue> accessBehavior =
            GetNextBehavior<IAccessPropertyBehavior<TValue>>();

         return accessBehavior.GetValue(vm);
      }

      public void SetValue(IBehaviorContext vm, object value) {
         IAccessPropertyBehavior<TValue> accessBehavior =
            GetNextBehavior<IAccessPropertyBehavior<TValue>>();

         ChangeValueRequest<TValue> request = value as ChangeValueRequest<TValue>;
         if (request != null) {
            ConversionResult<TValue> result = request.Converter.ConvertBack(request.NewValue);
            if (result.Successful) {
               vm.FieldValues.ClearField(_conversionErrorField);
               accessBehavior.SetValue(vm, result.Value);
            } else {
               vm.FieldValues.SetValue(_conversionErrorField, result.ErrorMessage);
               return;
            }
         } else {
            if (value is TValue) {
               accessBehavior.SetValue(vm, (TValue)value);
            } else {
               throw new ArgumentException(
                  ExceptionTexts.DisplayValueHasWrongType.FormatWith(
                     _propertyName, typeof(TValue).Name, value
                  ),
                  "value"
               );
            }
         }
      }

      public ValidationResult GetValidationResult(IBehaviorContext vm) {
         return vm.FieldValues.HasValue(_conversionErrorField) ?
            ValidationResult.Failure(vm.FieldValues.GetValue(_conversionErrorField)) :
            ValidationResult.Success();
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _conversionErrorField = context.DynamicFields.DefineField<string>(
            DynamicFieldGroups.ConversionErrorGroup
         );
      }
   }
}
