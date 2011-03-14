namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections;
   using System.Collections.Generic;

   internal sealed class ValidationCoordinatorBehavior : Behavior {
      private static readonly IVMCollection[] EmptyVMCollectionArray = new IVMCollection[0];

      public void Revalidate(IBehaviorContext context, IVMPropertyDescriptor property) {
         IEnumerable<IVMCollection> owners = GetOwnerCollectionsOfVM(context.VM);

         RefreshCollectionValidationResults(context, ValidationStep.DisplayValue, owners, property);
         RefreshCollectionValidationResults(context, ValidationStep.Value, owners, property);
      }

      /// <summary>
      ///   Refreshes the collection validation cache of the <paramref name="property"/>,
      ///   executes all validators for the it and returns the result.
      /// </summary>
      public ValidationResult ValidatePropertyWithFreshCollectionResults(
         IBehaviorContext context,
         ValidationStep step,
         IVMPropertyDescriptor property
      ) {
         IEnumerable<IVMCollection> owners = GetOwnerCollectionsOfVM(context.VM);
         RefreshCollectionValidationResults(context, step, owners, property);

         return ValidatePropertyWithCachedCollectionResults(context, step, property);
      }

      public ValidationResult ValidatePropertyWithCachedCollectionResults(
         IBehaviorContext context,
         ValidationStep step,
         IVMPropertyDescriptor property
      ) {
         return Validate(context, step, context.VM, property);
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

            var request = new ValidationRequest(step, validationTarget);

            ValidationResult result = InvokeValidationExecutors(context, request);

            var cache = collection.Behaviors.GetNextBehavior<CollectionValidationResultCacheBehavior>();
            cache.SetErrors(property, result.Errors);
         }
      }

      private ValidationResult Validate(
         IBehaviorContext context,
         ValidationStep step,
         IViewModel vm,
         IVMPropertyDescriptor property = null
      ) {
         var validationTarget = property != null ?
            Path.Empty.Prepend(property).Prepend(vm) :
            Path.Empty.Prepend(vm);

         var request = new ValidationRequest(step, validationTarget);

         var first = InvokeValidationExecutors(context, request);
         var second = GetCachedCollectionResults(vm, property);

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