﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class DisplayValueValidationBehavior : Behavior, IAccessPropertyBehavior, IValidationBehavior {
      private FieldDefinition<string> _validationErrorField;
      private List<Func<object, ValidationResult>> _validations = new List<Func<object, ValidationResult>>();

      object IAccessPropertyBehavior.GetValue(IBehaviorContext vm) {
         return GetNextBehavior<IAccessPropertyBehavior>().GetValue(vm);
      }

      void IAccessPropertyBehavior.SetValue(IBehaviorContext vm, object value) {
         foreach (Func<object, ValidationResult> validation in _validations) {
            ValidationResult result = validation(value);
            if (!result.Successful) {
               vm.FieldValues.SetValue(_validationErrorField, result.ErrorMessage);
               return;
            }
         }

         vm.FieldValues.ClearField(_validationErrorField);
         GetNextBehavior<IAccessPropertyBehavior>().SetValue(vm, value);
      }

      ValidationResult IValidationBehavior.GetValidationResult(IBehaviorContext vm) {
         string errorMessage;
         return vm.FieldValues.TryGetValue(_validationErrorField, out errorMessage) ?
            ValidationResult.Failure(errorMessage) :
            ValidationResult.Success();
      }

      internal void Add(Func<object, ValidationResult> validation) {
         Contract.Requires(validation != null);
         _validations.Add(validation);
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _validationErrorField = context.DynamicFields.DefineField<string>(
            DynamicFieldGroups.DisplayValueValidationErrorGroup
         );
      }
   }
}