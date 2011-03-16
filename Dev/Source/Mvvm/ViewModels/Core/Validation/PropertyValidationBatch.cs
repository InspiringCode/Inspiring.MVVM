using System;
namespace Inspiring.Mvvm.ViewModels.Core.Validation {

   internal sealed class PropertyValidationBatch {
      public PropertyValidationBatch(IVMPropertyDescriptor property) {

      }


      public ValidationResult ExecuteViewModelValidation(
         ValidationStep step,
         IViewModel viewModel
      ) {
         throw new NotImplementedException();
      }

      public ValidationResult ExecutePropertyValidation(
         ValidationStep step,
         IViewModel viewModel,
         IVMPropertyDescriptor property
      ) {
         throw new NotImplementedException();
      }

      //private ValidationResult ExecutePropertyValidation(ValidationStep step, IViewModel viewModel, IVMPropertyDescriptor property) {
      //   throw new NotImplementedException();
      //}

      //public ValidationResult Validate(IViewModel viewModel

      //private ValidationResult InvokeValidationExecutors(IViewModel viewModel) {
      //   //viewModel.Descriptor.Behaviors.TryCall<IValidationExecutorBehavior>(
      //}

      //private ValidationResult GetCollectionResults(IViewModel item, ValidationStep step) {

      //}

      //private void X(IViewModel item) {

      //}

      private ValidationResult InvokeValidationExecutors(
         ValidationStep step,
         IViewModel viewModel,
         IVMPropertyDescriptor property = null
      ) {
         var requestPath = Path.Empty
            .Append(viewModel);

         if (property != null) {
            requestPath = requestPath.Append(property);
         }

         var request = new ValidationRequest(step, requestPath);

         return ExecuteValidationRequest(request, viewModel);
      }


      private ValidationResult InvokeCollectionValidationExecutors(CollectionResultKey key) {
         var requestPath = Path.Empty
            .Append(key.Collection.Owner)
            .Append(key.Collection);

         if (key.Property != null) {
            requestPath = requestPath.Append(key.Property);
         }

         var request = new ValidationRequest(key.Step, requestPath);

         return ExecuteValidationRequest(request, key.Collection.Owner);
      }

      private ValidationResult ExecuteValidationRequest(ValidationRequest request, IViewModel requestTarget) {
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



      private void Foo() {
         // 1. step
         // 2. path: parent, coll, property

         //new ValidationRequest(
      }



      private ValidationResult GetCollectionResults(
         IViewModel item,
         IVMPropertyDescriptor property,
         ValidationStep step
      ) {
         throw new NotImplementedException();
      }

      private class ResultCache {

         public ValidationResult Result { get; private set; }
      }

      private class CollectionResultKey {
         public IVMCollection Collection { get; private set; }
         public IVMPropertyDescriptor Property { get; private set; }
         public ValidationStep Step { get; private set; }
      }
   }
}
