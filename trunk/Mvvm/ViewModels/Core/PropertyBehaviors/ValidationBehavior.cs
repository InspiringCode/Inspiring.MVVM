﻿namespace Inspiring.Mvvm.ViewModels.Core {
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
      private VMPropertyBase<TValue> _property;
      private List<Action<ValidationEventArgs>> _validators
         = new List<Action<ValidationEventArgs>>();

      public void Add(Action<ValidationEventArgs> validator) {
         Contract.Requires(validator != null);
         _validators.Add(validator);
      }

      public TValue GetValue(IBehaviorContext vm) {
         return GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         var oldResult = GetValidationResult(vm);

         var args = new ValidationEventArgs(_property, value, vm.VM);
         OnValidating(args);
         vm.OnValidating(args);
         vm.OnValidated(args);

         if (args.Errors.Count > 0) {
            vm.FieldValues.SetValue(_errorMessageField, args.Errors.First());
         } else {
            this.SetNextValue(vm, value);
            vm.FieldValues.ClearField(_errorMessageField);
         }

         var newResult = GetValidationResult(vm);

         if (!newResult.Equals(oldResult)) {
            vm.ValidationStateChanged(_property);
         }
      }

      public ValidationResult GetValidationResult(IBehaviorContext vm) {
         //bool hasErrorsBefore = vm.FieldValues.HasValue(_errorMessageField);

         //// HACK to make sure that validation is always current (search for more efficient ways).
         //TValue value = GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);

         //var parameter = new ValidationParameter<TValue>(value, vm.VM);

         //ValidationResult result = _validators
         //   .Select(v => v(parameter))
         //   .FirstOrDefault(res => !res.Successful);

         //if (result != null) {
         //   vm.FieldValues.SetValue(_errorMessageField, result.ErrorMessage);
         //}

         //bool hasErrors = vm.FieldValues.HasValue(_errorMessageField);

         //if (hasErrors != hasErrorsBefore) {
         //   vm.ValidationStateChanged(_property);
         //}

         return vm.FieldValues.HasValue(_errorMessageField) ?
            ValidationResult.Failure(vm.FieldValues.GetValue(_errorMessageField)) :
            ValidationResult.Success();
      }

      public void HandlePropertyChanging(IBehaviorContext vm) {
         //TValue value = GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);

         //var parameter = new ValidationParameter<TValue>(value, vm.VM);

         //ValidationResult result = _validators
         //   .Select(v => v(parameter))
         //   .FirstOrDefault(res => !res.Successful);

         //if (result != null) {
         //   vm.FieldValues.SetValue(_errorMessageField, result.ErrorMessage);
         //} else {
         //   vm.FieldValues.ClearField(_errorMessageField);
         //}
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _errorMessageField = context.DynamicFields.DefineField<string>(
            DynamicFieldGroups.ValidationErrorGroup
         );
         _property = (VMPropertyBase<TValue>)context.Property;
      }

      private void OnValidating(ValidationEventArgs args) {
         _validators.ForEach(v => v(args));
      }
   }
}
