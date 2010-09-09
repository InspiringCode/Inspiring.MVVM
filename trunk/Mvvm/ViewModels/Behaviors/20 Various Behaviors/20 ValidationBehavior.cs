namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System;

   internal sealed class ValidationBehavior<TValue> : VMPropertyBehavior, IAccessPropertyBehavior<TValue>, IValidationBehavior {
      private FieldDefinition<string> _errorMessageField;

      public override BehaviorPosition Position {
         get { return BehaviorPosition.Validator; }
      }

      public void Add(Func<TValue, ValidationResult> validation) {
         throw new NotImplementedException();
      }

      public TValue GetValue(IBehaviorContext vm) {
         AssertInitialized();
         return GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         AssertInitialized();

         throw new NotImplementedException();
      }

      public ValidationResult GetValidationResult(IBehaviorContext vm) {
         AssertInitialized();

         throw new NotImplementedException();
      }

      protected override void OnDefineDynamicFields(FieldDefinitionCollection fields) {
         base.OnDefineDynamicFields(fields);
         _errorMessageField = fields.DefineField<string>(DynamicFieldGroups.ValidationErrorGroup);
      }
   }
}
