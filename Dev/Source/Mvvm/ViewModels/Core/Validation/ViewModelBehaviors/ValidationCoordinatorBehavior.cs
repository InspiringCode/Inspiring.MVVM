namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections;
   using System.Collections.Generic;

   internal sealed class ValidationCoordinatorBehavior : Behavior {
      private static readonly IVMCollection[] EmptyVMCollectionArray = new IVMCollection[0];

      public void Revalidate(IBehaviorContext context, IVMPropertyDescriptor property) {
         IEnumerable<IVMCollection> owners = GetOwnerCollectionsOfVM(context.VM);

         RefreshCollectionValidationResults(context, ValidationStep.DisplayValue, owners, property);
         RefreshCollectionValidationResults(context, ValidationStep.Value, owners, property);

         property.Behaviors.GetNextBehavior<IRevalidationBehavior>().Revalidate(context);
      }

      public ValidationResult ValidateProperty(IBehaviorContext context, ValidationRequest request) {
         if (request.Trigger == ValidationTrigger.PropertyChange) {
            IEnumerable<IVMCollection> owners = GetOwnerCollectionsOfVM(context.VM);
            RefreshCollectionValidationResults(context, request.Step, owners, request.TargetProperty);
         }
         return Validate(context, request);
      }

      private IEnumerable<IVMCollection> GetOwnerCollectionsOfVM(IViewModel vm) {
         return vm.Kernel.OwnerCollection != null ?
            new[] { vm.Kernel.OwnerCollection } :
            EmptyVMCollectionArray;
      }

      private void RefreshCollectionValidationResults(
         IBehaviorContext context,
         ValidationStep step,
         IEnumerable<IVMCollection> ownerCollections,
         IVMPropertyDescriptor property
      ) {
         foreach (IVMCollection collection in ownerCollections) {
            Path validationTarget = Path.Empty
               .Prepend(property)
               .Prepend(collection)
               .Prepend(collection.Owner);

            var request = new ValidationRequest(
               ValidationTrigger.CollectionValidation,
               step,
               validationTarget
            );

            ValidationResult result = InvokeValidationExecutors(context, request);

            var cache = collection.Behaviors.GetNextBehavior<CollectionValidationResultCacheBehavior>();
            cache.SetErrors(property, result.Errors);
         }
      }

      private ValidationResult Validate(
         IBehaviorContext context,
         ValidationRequest request
      ) {
         var first = InvokeValidationExecutors(context, request);
         var second = GetCachedCollectionResults(request.Target, request.TargetProperty);

         return ValidationResult.Join(first, second);
      }

      private ValidationResult InvokeValidationExecutors(
         IBehaviorContext context,
         ValidationRequest request
      ) {
         IValidationExecutorBehavior firstExecutor;
         return TryGetBehavior(out firstExecutor) ?
            firstExecutor.Validate(context, request) :
            ValidationResult.Valid;
      }

      private ValidationResult GetCachedCollectionResults(IViewModel item, IVMPropertyDescriptor property) {
         IVMCollection owner = item.Kernel.OwnerCollection;

         if (owner != null) {
            var cache = owner.Behaviors.GetNextBehavior<CollectionValidationResultCacheBehavior>();
            IEnumerable<ValidationError> errors = cache.GetItemErrors(item, property);
            return new ValidationResult(errors);
         }

         return ValidationResult.Valid;
      }
   }
}