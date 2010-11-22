namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;

   internal sealed class DisplayValueAccessorBehavior<TValue> : Behavior, IDisplayValueAccessorBehavior, IValidationBehavior {
      private FieldDefinition<string> _conversionErrorField;
      private string _propertyName;

      public object GetDisplayValue(IBehaviorContext vm) {
         IPropertyAccessorBehavior<TValue> accessBehavior =
            GetNextBehavior<IPropertyAccessorBehavior<TValue>>();

         return accessBehavior.GetValue(vm, ValueStage.PostValidation);
      }

      public void SetDisplayValue(IBehaviorContext vm, object value) {
         IPropertyAccessorBehavior<TValue> accessBehavior =
            GetNextBehavior<IPropertyAccessorBehavior<TValue>>();

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
            // TODO: Tidy up this if...
            if (value is TValue || (value == null && TypeService.CanAssignNull(typeof(TValue)))) {
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
         if (vm.FieldValues.HasValue(_conversionErrorField)) {
            return ValidationResult.Failure(vm.FieldValues.GetValue(_conversionErrorField));
         }
         IValidationBehavior next;
         return TryGetBehavior(out next) ?
            next.GetValidationResult(vm) :
            ValidationResult.Success();
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _conversionErrorField = context.Fields.DefineField<string>(
            DynamicFieldGroups.ConversionErrorGroup
         );
         _propertyName = context.Property.PropertyName;
      }
   }
}
