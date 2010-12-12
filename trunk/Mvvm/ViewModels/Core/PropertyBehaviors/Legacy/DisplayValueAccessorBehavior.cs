namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;

   namespace New {
      internal sealed class DisplayValueAccessorBehavior<TValue> :
         Behavior,
         IBehaviorInitializationBehavior,
         IDisplayValueAccessorBehavior,
         IValidationBehavior {

         public void Initialize(BehaviorInitializationContext context) {
            throw new NotImplementedException();
         }

         public object GetDisplayValue(IBehaviorContext vm) {
            throw new NotImplementedException();
         }

         public void SetDisplayValue(IBehaviorContext vm, object value) {
            throw new NotImplementedException();
         }

         public ValidationResult GetValidationResult(IBehaviorContext vm) {
            throw new NotImplementedException();
         }
      }
   }


   internal sealed class DisplayValueAccessorBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IDisplayValueAccessorBehavior,
      IValidationBehavior {

      private FieldDefinition<string> _conversionErrorField;
      private string _propertyName;

      public object GetDisplayValue(IBehaviorContext vm) {
         IValueAccessorBehavior<TValue> accessBehavior =
            GetNextBehavior<IValueAccessorBehavior<TValue>>();

         return accessBehavior.GetValue(vm, ValueStage.PreValidation);
      }

      public void SetDisplayValue(IBehaviorContext vm, object value) {
         IValueAccessorBehavior<TValue> accessBehavior =
            GetNextBehavior<IValueAccessorBehavior<TValue>>();

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

      public void Initialize(BehaviorInitializationContext context) {
         _conversionErrorField = context.Fields.DefineField<string>(
            DynamicFieldGroups.ConversionErrorGroup
         );
         _propertyName = context.Property.PropertyName;

         this.InitializeNext(context);
      }
   }
}
