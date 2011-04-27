﻿namespace Inspiring.Mvvm.ViewModels.Core {

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

      public void Revalidate(IBehaviorContext context, CollectionResultCache cache) {
         ValidationController controller = _validationController.Get(context);

         var result = controller.GetResult(_step, context.VM, _property);

         bool valueWasInvalid = _invalidValueCache.HasValue(context);
         bool valueIsNotInvalidAnymore = result.IsValid;

         if (valueWasInvalid && valueIsNotInvalidAnymore) {
            TValue previouslyInvalidValue = _invalidValueCache.Get(context);

            _invalidValueCache.Clear(context);
            SetValueNext(context, previouslyInvalidValue);
         } else {
            this.PropertyRevalidateNext(context, cache);
         }

         _resultManager.UpdateValidationResult(context, result);

         //var result = ValidationOperation.PerformPropertyValidation(cache, _step, context.VM, _property);

         //bool valueWasInvalid = _invalidValueCache.HasValue(context);
         //bool valueIsNotInvalidAnymore = result.IsValid;

         //if (valueWasInvalid && valueIsNotInvalidAnymore) {
         //   TValue previouslyInvalidValue = _invalidValueCache.Get(context);

         //   _invalidValueCache.Clear(context);
         //   SetValueNext(context, previouslyInvalidValue);
         //} else {
         //   this.PropertyRevalidateNext(context, cache);
         //}

         //_resultManager.UpdateValidationResult(context, result);
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


      protected void SetValueIfValidationSucceeds(IBehaviorContext context, TValue value) {
         CachePotentiallyInvalidValue(context, value);

         if (_validationController.HasValue(context)) {
            ValidationController controller = _validationController.Get(context);
            SetValueIfValidationSucceeds(context, controller);
         } else {
            var controller = new ValidationController();

            controller.ManuallyBeginValidation(context.VM, _property);
            SetValueIfValidationSucceeds(context, controller);
            controller.ManuallyEndValidation(context.VM, _property);

            controller.ProcessPendingValidations();
         }
      }

      protected abstract TValue GetValueNext(IBehaviorContext context);

      protected abstract void SetValueNext(IBehaviorContext context, TValue value);


      private void SetValueIfValidationSucceeds(IBehaviorContext context, ValidationController controller) {
         TValue newValue = _invalidValueCache.Get(context);

         ValidationResult result = controller.GetResult(_step, context.VM, _property);

         if (result.IsValid) {
            _invalidValueCache.Clear(context);
            SetValueNext(context, newValue);
         }

         _resultManager.UpdateValidationResult(context, result);


         //TValue previousValue = GetValueNext(context);

         //CachePotentiallyInvalidValue(context, newValue);

         //var result = ValidationOperation.PerformPropertyValidation(
         //   new CollectionResultCache(),
         //   _step,
         //   context.VM,
         //   _property
         //);

         //if (result.IsValid) {
         //   _invalidValueCache.Clear(context);
         //   SetValueNext(context, newValue);
         //}

         //_resultManager.UpdateValidationResult(context, result);
      }

      private void CachePotentiallyInvalidValue(IBehaviorContext context, TValue value) {
         _invalidValueCache.Set(context, value);
      }
   }
}