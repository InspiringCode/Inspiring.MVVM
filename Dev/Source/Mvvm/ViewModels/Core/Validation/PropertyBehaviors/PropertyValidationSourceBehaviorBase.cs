namespace Inspiring.Mvvm.ViewModels.Core {

   internal abstract class PropertyValidationSourceBehaviorBase<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValidationResultProviderBehavior,
      IRevalidationBehavior,
      IHandlePropertyChangedBehavior {

      private static readonly FieldDefinitionGroup ValidationResultGroup = new FieldDefinitionGroup();
      private static readonly FieldDefinitionGroup InvalidValueGroup = new FieldDefinitionGroup();

      private readonly ValidationStep _step;
      private ValidationResultManager _resultManager;
      private DynamicFieldAccessor<TValue> _invalidValueCache;
      private IVMPropertyDescriptor _property;

      protected PropertyValidationSourceBehaviorBase(ValidationStep step) {
         _step = step;
      }

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         _resultManager = new ValidationResultManager(context, ValidationResultGroup);
         _invalidValueCache = new DynamicFieldAccessor<TValue>(context, InvalidValueGroup);
         this.InitializeNext(context);
      }

      public ValidationResult GetValidationResult(IBehaviorContext context) {
         var next = this.GetValidationResultNext(context);
         var result = _resultManager.GetValidationResult(context);
         return ValidationResult.Join(result, next);
      }

      public void Revalidate(IBehaviorContext context, CollectionResultCache cache) {
         var result = ValidationOperation.PerformPropertyValidation(cache, _step, context.VM, _property);

         bool valueWasInvalid = _invalidValueCache.HasValue(context);
         bool valueIsNotInvalidAnymore = result.IsValid;

         if (valueWasInvalid && valueIsNotInvalidAnymore) {
            TValue previouslyInvalidValue = _invalidValueCache.Get(context);

            _invalidValueCache.Clear(context);
            SetValueNext(context, previouslyInvalidValue);
         } else {
            this.RevalidateNext(context, cache);
         }

         _resultManager.UpdateValidationResult(context, result);
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
         TValue previousValue = GetValueNext(context);

         CachePotentiallyInvalidValue(context, value);

         var result = ValidationOperation.PerformPropertyValidation(
            new CollectionResultCache(),
            _step,
            context.VM,
            _property
         );

         if (result.IsValid) {
            _invalidValueCache.Clear(context);
            SetValueNext(context, value);
         }

         _resultManager.UpdateValidationResult(context, result);
      }

      protected abstract TValue GetValueNext(IBehaviorContext context);

      protected abstract void SetValueNext(IBehaviorContext context, TValue value);

      private void CachePotentiallyInvalidValue(IBehaviorContext context, TValue value) {
         _invalidValueCache.Set(context, value);
      }
   }
}