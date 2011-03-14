namespace Inspiring.Mvvm.ViewModels.Core.Validation.PropertyBehaviors {

   internal sealed class PropertyValidationBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue> {

      private static readonly FieldDefinitionGroup ValidationResultGroup = new FieldDefinitionGroup();
      private static readonly FieldDefinitionGroup InvalidValueGroup = new FieldDefinitionGroup();

      private DynamicFieldAccessor<ValidationResult> _resultField;
      private DynamicFieldAccessor<TValue> _invalidValueCache;
      private IVMPropertyDescriptor _property;

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         _resultField = new DynamicFieldAccessor<ValidationResult>(context, ValidationResultGroup);
         _invalidValueCache = new DynamicFieldAccessor<TValue>(context, InvalidValueGroup);
         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         CachePotentiallyInvalidValue(context, value);

         var coordinator = context
            .VM
            .Descriptor
            .Behaviors
            .GetNextBehavior<ValidationCoordinatorBehavior>();

         ValidationResult result = coordinator.ValidatePropertyWithFreshCollectionResults(
            context,
            ValidationStep.Value,
            _property
         );

         if (result.IsValid) {
            _invalidValueCache.Clear(context);
            this.SetValueNext(context, value);
         }

         var previousResult = GetValidationResult(context);
         if (!result.Equals(previousResult)) {
            SetValidationResult(context, result);

            var args = new ChangeArgs(
               ChangeType.ValidationStateChanged,
               changedVM: context.VM,
               changedProperty: _property
            );

            context.NotifyChange(args);
         }
      }

      private void CachePotentiallyInvalidValue(IBehaviorContext context, TValue value) {
         _invalidValueCache.Set(context, value);
      }

      private ValidationResult GetValidationResult(IBehaviorContext context) {
         return _resultField.GetWithDefault(context, ValidationResult.Valid);
      }

      private void SetValidationResult(IBehaviorContext context, ValidationResult result) {
         if (result.IsValid) {
            _resultField.Clear(context);
         } else {
            _resultField.Set(context, result);
         }
      }
   }
}