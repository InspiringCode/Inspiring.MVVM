using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal abstract class PropertyValidationSourceBehaviorBase<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValidationStateProviderBehavior,
      IRevalidationBehavior,
      IHandlePropertyChangedBehavior {

      private static readonly FieldDefinitionGroup ValidationResultGroup = new FieldDefinitionGroup();
      private static readonly FieldDefinitionGroup InvalidValueGroup = new FieldDefinitionGroup();

      private readonly ValidationStep _step;
      private DynamicFieldAccessor<ValidationResult> _resultField;
      private DynamicFieldAccessor<TValue> _invalidValueCache;
      private IVMPropertyDescriptor _property;

      protected PropertyValidationSourceBehaviorBase(ValidationStep step) {
         _step = step;
      }

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         _resultField = new DynamicFieldAccessor<ValidationResult>(context, ValidationResultGroup);
         _invalidValueCache = new DynamicFieldAccessor<TValue>(context, InvalidValueGroup);
         this.InitializeNext(context);
      }

      public ValidationResult GetValidationState(IBehaviorContext context) {
         var next = this.GetValidationStateNext(context);
         var result = GetValidationResult(context);
         return ValidationResult.Join(result, next);
      }

      public ValidationResult GetDescendantsValidationState(IBehaviorContext context) {
         return this.GetDescendantsValidationStateNext(context);
      }

      public void Revalidate(IBehaviorContext context) {
         var result = InvokeValidation(context, ValidationTrigger.Revalidate);

         bool valueWasInvalid = _invalidValueCache.HasValue(context);
         bool valueIsNotInvalidAnymore = result.IsValid;

         if (valueWasInvalid && valueIsNotInvalidAnymore) {
            TValue previouslyInvalidValue = _invalidValueCache.Get(context);

            _invalidValueCache.Clear(context);
            SetValueNext(context, previouslyInvalidValue);
         } else {
            this.RevalidateNext(context);
         }

         UpdateValidationResult(context, result);
      }

      // TODO: Remove
      public void Revalidate(IBehaviorContext context, ValidationContext validationContext, ValidationMode mode) {
         throw new NotImplementedException();
      }

      public void HandlePropertyChanged(IBehaviorContext context) {
         _invalidValueCache.Clear(context);
         this.HandlePropertyChangedNext(context);
      }

      protected TValue GetInvalidValueOrNext(IBehaviorContext context) {
         return _invalidValueCache.HasValue(context) ?
            _invalidValueCache.Get(context) :
            GetValueNext(context);
      }

      protected void SetValueIfValidationSucceeds(IBehaviorContext context, TValue value) {
         CachePotentiallyInvalidValue(context, value);

         var result = InvokeValidation(context, ValidationTrigger.PropertyChange);

         if (result.IsValid) {
            _invalidValueCache.Clear(context);
            SetValueNext(context, value);
         }

         UpdateValidationResult(context, result);
      }

      protected abstract TValue GetValueNext(IBehaviorContext context);

      protected abstract void SetValueNext(IBehaviorContext context, TValue value);

      private static ValidationCoordinatorBehavior GetCoordinator(IBehaviorContext context) {
         return context
            .VM
            .Descriptor
            .Behaviors
            .GetNextBehavior<ValidationCoordinatorBehavior>();
      }

      private ValidationResult InvokeValidation(IBehaviorContext context, ValidationTrigger trigger) {
         var coordinator = GetCoordinator(context);
         var request = new ValidationRequest(trigger, _step, context.VM, _property);
         return coordinator.ValidateProperty(context, request);
      }

      private void CachePotentiallyInvalidValue(IBehaviorContext context, TValue value) {
         _invalidValueCache.Set(context, value);
      }

      private void UpdateValidationResult(IBehaviorContext context, ValidationResult result) {
         var previousResult = GetValidationResult(context);
         if (!result.Equals(previousResult)) {
            SetOrClearValidationResult(context, result);

            var args = new ChangeArgs(
               ChangeType.ValidationStateChanged,
               changedVM: context.VM,
               changedProperty: _property
            );

            context.NotifyChange(args);
         }
      }

      private ValidationResult GetValidationResult(IBehaviorContext context) {
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