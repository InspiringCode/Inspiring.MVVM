namespace Inspiring.Mvvm.ViewModels.Core {

   internal abstract class PropertyValidationSourceBehaviorBase<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValidationResultProviderBehavior,
      IPropertyRevalidationBehavior,
      IHandlePropertyChangedBehavior {

      private static readonly FieldDefinitionGroup ValidationResultGroup = new FieldDefinitionGroup();
      private static readonly FieldDefinitionGroup InvalidValueGroup = new FieldDefinitionGroup();
      private static readonly FieldDefinitionGroup ValidationControllerGroup = new FieldDefinitionGroup();

      private readonly ValidationStep _step;
      private ValidationResultManager _resultManager;
      private DynamicFieldAccessor<TValue> _invalidValueCache;
      private DynamicFieldAccessor<ValidationController> _validationController;
      private IVMPropertyDescriptor _property;

      protected PropertyValidationSourceBehaviorBase(ValidationStep step) {
         _step = step;
      }

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         _resultManager = new ValidationResultManager(context, ValidationResultGroup);
         _invalidValueCache = new DynamicFieldAccessor<TValue>(context, InvalidValueGroup);
         _validationController = new DynamicFieldAccessor<ValidationController>(context, ValidationControllerGroup);
         this.InitializeNext(context);
      }

      public ValidationResult GetValidationResult(IBehaviorContext context) {
         var next = this.GetValidationResultNext(context);
         var result = _resultManager.GetValidationResult(context);
         return ValidationResult.Join(result, next);
      }

      public void BeginValidation(IBehaviorContext context, ValidationController controller) {
         _validationController.Set(context, controller);
         this.BeginValidationNext(context, controller);
      }

      public void Revalidate(IBehaviorContext context) {
         ValidationController controller = _validationController.Get(context);
         var validationResult = controller.GetResult(_step, context.VM, _property);

         bool valueWasInvalid = _invalidValueCache.HasValue(context);
         bool valueIsNotInvalidAnymore = validationResult.IsValid;

         if (valueWasInvalid && valueIsNotInvalidAnymore) {
            TValue previouslyInvalidValue = _invalidValueCache.Get(context);

            _invalidValueCache.Clear(context);
            SetValueNext(context, previouslyInvalidValue);
         } else {
            this.PropertyRevalidateNext(context);
         }

         _resultManager.UpdateValidationResult(context, validationResult);
      }

      public void EndValidation(IBehaviorContext context) {
         _validationController.Clear(context);
         this.EndValidationNext(context);
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
      
      protected void SetValueNextIfValidationSucceeds(IBehaviorContext context, TValue value) {
         CachePotentiallyInvalidValue(context, value);

         ValidationController controller;
         if (_validationController.TryGet(context, out controller)) {
            ValidateAndSetValueWithExistingController(context, controller);
         } else {
            ValidateAndSetValueWithNewController(context);
         }
      }

      protected abstract TValue GetValueNext(IBehaviorContext context);

      protected abstract void SetValueNext(IBehaviorContext context, TValue value);

      private void ValidateAndSetValueWithNewController(IBehaviorContext context) {
         var controller = new ValidationController();

         controller.ManuallyBeginValidation(context.VM, _property);
         ValidateAndSetValueWithExistingController(context, controller);
         controller.ManuallyEndValidation(context.VM, _property);

         controller.ProcessPendingValidations();
      }

      private void ValidateAndSetValueWithExistingController(IBehaviorContext context, ValidationController controller) {
         ValidationResult validationResult = controller.GetResult(_step, context.VM, _property);

         if (validationResult.IsValid) {
            TValue newValue = _invalidValueCache.Get(context);
            _invalidValueCache.Clear(context);
            SetValueNext(context, newValue);
         }

         _resultManager.UpdateValidationResult(context, validationResult);
      }

      private void CachePotentiallyInvalidValue(IBehaviorContext context, TValue value) {
         _invalidValueCache.Set(context, value);
      }
   }
}