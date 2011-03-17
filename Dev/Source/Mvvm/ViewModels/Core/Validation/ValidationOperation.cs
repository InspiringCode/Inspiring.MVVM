namespace Inspiring.Mvvm.ViewModels.Core.Validation {
   using System;

   internal sealed class ValidationOperation {
      private CollectionResultCache _cache;
      private ValidationStep _step;
      private IViewModel _viewModel;
      private IVMPropertyDescriptor _property;

      private ValidationOperation(
         CollectionResultCache cache,
         ValidationStep step,
         IViewModel viewModel,
         IVMPropertyDescriptor property = null
      ) {
         _cache = cache;
         _step = step;
         _viewModel = viewModel;
         _property = property;
      }

      public static ValidationResult PerformPropertyValidation(
         CollectionResultCache cache,
         ValidationStep step,
         IViewModel viewModel,
         IVMPropertyDescriptor property
      ) {
         return new ValidationOperation(cache, step, viewModel, property).Execute();
      }

      public static ValidationResult PerformViewModelValidation(
         CollectionResultCache cache,
         IViewModel viewModel
      ) {
         return new ValidationOperation(cache, ValidationStep.ViewModel, viewModel).Execute();
      }

      private ValidationResult Execute() {
         throw new NotImplementedException();
      }



      private ValidationResult InvokeValidationExecutors() {
         var requestPath = Path.Empty
            .Append(_viewModel);

         if (_property != null) {
            requestPath = requestPath.Append(_property);
         }

         var request = new ValidationRequest(_step, requestPath);
         return _viewModel.ExecuteValidationRequest(request);
         //return ExecuteRequest(request, _viewModel);
      }


      //private ValidationResult InvokeCollectionValidationExecutors(CollectionResultKey key) {
      //   var requestPath = Path.Empty
      //      .Append(key.Collection.Owner)
      //      .Append(key.Collection);

      //   if (key.Property != null) {
      //      requestPath = requestPath.Append(key.Property);
      //   }

      //   var request = new ValidationRequest(key.Step, requestPath);
      //   return key.Collection.Owner.ExecuteValidationRequest(request);

      //   //return ExecuteRequest(request, key.Collection.Owner);
      //}

      private ValidationResult ExecuteRequest(ValidationRequest request, IViewModel requestTarget) {
         IValidationExecutorBehavior executor;

         bool hasExecutors = requestTarget
            .Descriptor
            .Behaviors
            .TryGetBehavior(out executor);

         if (!hasExecutors) {
            return ValidationResult.Valid;
         }

         return executor.Validate(requestTarget.GetContext(), request);
      }
   }
}
