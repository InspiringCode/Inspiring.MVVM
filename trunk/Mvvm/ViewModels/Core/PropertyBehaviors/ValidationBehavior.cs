namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ValidationBehavior<TValue> : Behavior, IAccessPropertyBehavior<TValue>, IValidationBehavior {
      private FieldDefinition<string> _errorMessageField;

      public void Add(Func<TValue, ValidationResult> validation) {
         throw new NotImplementedException();
      }

      public TValue GetValue(IBehaviorContext vm) {
         return GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         throw new NotImplementedException();
      }

      public ValidationResult GetValidationResult(IBehaviorContext vm) {
         throw new NotImplementedException();
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _errorMessageField = context.DynamicFields.DefineField<string>(
            DynamicFieldGroups.ValidationErrorGroup
         );
      }
   }
}
