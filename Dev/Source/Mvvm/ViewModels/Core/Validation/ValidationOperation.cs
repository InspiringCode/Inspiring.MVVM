namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ValidationOperation {
      private CollectionResultCache _cache;
      private ValidationStep _step;
      private IViewModel _viewModel;
      private IVMPropertyDescriptor _property;

      private ValidationOperation(
         CollectionResultCache cache,
         ValidationStep step,
         IViewModel viewModel,
         IVMPropertyDescriptor property
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
         return new ValidationOperation(cache, ValidationStep.ViewModel, viewModel, null).Execute();
      }

      private ValidationResult Execute() {
         var result = InvokeValidationExecutors();

         var collectionResults = _cache.GetCollectionValidationResults(
            _step,
            _viewModel,
            _property
         );

         return ValidationResult.Join(result, collectionResults);
      }

      private ValidationResult InvokeValidationExecutors() {
         var requestPath = Path.Empty
            .Append(_viewModel);

         if (_property != null) {
            requestPath = requestPath.Append(_property);
         }

         var request = new ValidationRequest(_step, requestPath);
         return _viewModel.ExecuteValidationRequest(request);
      }
   }
}
