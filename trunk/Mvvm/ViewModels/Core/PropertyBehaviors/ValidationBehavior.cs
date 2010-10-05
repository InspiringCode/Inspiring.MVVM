namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   internal sealed class ValidationBehavior<TValue> : 
      Behavior,
      IAccessPropertyBehavior<TValue>,
      IValidationBehavior,
      IHandlePropertyChangingBehavior {

      private FieldDefinition<string> _errorMessageField;
      private List<Func<ValidationParameter<TValue>, ValidationResult>> _validators =
         new List<Func<ValidationParameter<TValue>, ValidationResult>>();

      public void Add(Func<ValidationParameter<TValue>, ValidationResult> validator) {
         Contract.Requires(validator != null);
         _validators.Add(validator);
      }

      public TValue GetValue(IBehaviorContext vm) {
         return GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         var parameter = new ValidationParameter<TValue>(value, vm.VM);

         ValidationResult result = _validators
            .Select(v => v(parameter))
            .FirstOrDefault(res => !res.Successful);

         if (result != null) {
            vm.FieldValues.SetValue(_errorMessageField, result.ErrorMessage);
         } else {
            GetNextBehavior<IAccessPropertyBehavior<TValue>>().SetValue(vm, value);
            vm.FieldValues.ClearField(_errorMessageField);
         }
      }

      public ValidationResult GetValidationResult(IBehaviorContext vm) {
         // HACK to make sure that validation is always current (search for more efficient ways).
         TValue value = GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);
         
         var parameter = new ValidationParameter<TValue>(value, vm.VM);

         ValidationResult result = _validators
            .Select(v => v(parameter))
            .FirstOrDefault(res => !res.Successful);

         if (result != null) {
            vm.FieldValues.SetValue(_errorMessageField, result.ErrorMessage);
         }

         return vm.FieldValues.HasValue(_errorMessageField) ?
            ValidationResult.Failure(vm.FieldValues.GetValue(_errorMessageField)) :
            ValidationResult.Success();
      }

      public void HandlePropertyChanging(IBehaviorContext vm) {
         TValue value = GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);
         
         var parameter = new ValidationParameter<TValue>(value, vm.VM);

         ValidationResult result = _validators
            .Select(v => v(parameter))
            .FirstOrDefault(res => !res.Successful);

         if (result != null) {
            vm.FieldValues.SetValue(_errorMessageField, result.ErrorMessage);
         } else {
            vm.FieldValues.ClearField(_errorMessageField);
         }
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _errorMessageField = context.DynamicFields.DefineField<string>(
            DynamicFieldGroups.ValidationErrorGroup
         );
      }
   }

   internal class ValidationParameter<TValue> {
      public ValidationParameter(TValue value, ViewModel vm) {
         Contract.Requires(vm != null);

         Value = value;
         VM = vm;
      }

      public TValue Value { get; private set; }
      public ViewModel VM { get; private set; }
   }
}
