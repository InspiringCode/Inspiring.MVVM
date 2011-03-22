namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class ValidationCoordinatorBehavior : Behavior {
      private static readonly IVMCollection[] EmptyVMCollectionArray = new IVMCollection[0];

      private DynamicFieldAccessor<ValidationResult> _validationResultField;

      public void Initialize(BehaviorInitializationContext context) {
         _validationResultField = new DynamicFieldAccessor<ValidationResult>(
            context,
            ViewModel.GeneralFieldGroup
         );

         this.InitializeNext(context);
      }

      public void Revalidate(IBehaviorContext context, IVMPropertyDescriptor property) {
         IEnumerable<IVMCollection> owners = GetOwnerCollectionsOfVM(context.VM);

         RefreshCollectionValidationResults(context, ValidationStep.DisplayValue, owners, property);
         RefreshCollectionValidationResults(context, ValidationStep.Value, owners, property);

         property.Behaviors.GetNextBehavior<IRevalidationBehavior>().Revalidate(context);
      }

      public void Revalidate(IBehaviorContext context, IVMCollection collection) {
         var itemDescriptor = collection.GetItemDescriptor();
         var owners = new[] { collection };

         itemDescriptor.Properties.ForEach(p => {
            RefreshCollectionValidationResults(context, ValidationStep.DisplayValue, owners, p);
            RefreshCollectionValidationResults(context, ValidationStep.Value, owners, p);
         });

         foreach (IViewModel item in collection) {
            ValidateViewModel(context, item);
         }
      }

      public void Revalidate(IBehaviorContext context) {
         foreach (var property in context.VM.Descriptor.Properties) {
            Revalidate(context, property);
         }
         ExecuteViewModelValidators(context, context.VM);
      }

      private void ValidateViewModel(IBehaviorContext context, IViewModel viewModel) {
         foreach (IVMPropertyDescriptor property in viewModel.Descriptor.Properties) {
            property
               .Behaviors
               .GetNextBehavior<IRevalidationBehavior>()
               .Revalidate(context);
         }

         ExecuteViewModelValidators(context, viewModel);
      }

      private void ExecuteViewModelValidators(IBehaviorContext context, IViewModel viewModel) {
         ValidationRequest request = new ValidationRequest(
            ValidationTrigger.Revalidate,
            ValidationStep.ViewModel,
            viewModel
         );

         ValidationResult result = InvokeValidationExecutors(context, request);

         var previousResult = _validationResultField.GetWithDefault(context, ValidationResult.Valid);
         if (!result.Equals(previousResult)) {
            if (result.IsValid) {
               _validationResultField.Clear(context);
            } else {
               _validationResultField.Set(context, result);
            }

            ChangeArgs args = new ChangeArgs(ChangeType.ValidationStateChanged, viewModel);
            context.NotifyChange(args);
         }
      }

      public ValidationResult ValidateProperty(IBehaviorContext context, ValidationRequest request) {
         if (request.Trigger == ValidationTrigger.PropertyChange) {
            IEnumerable<IVMCollection> owners = GetOwnerCollectionsOfVM(context.VM);
            RefreshCollectionValidationResults(context, request.Step, owners, request.TargetProperty);
         }
         return Validate(context, request);
      }

      private IEnumerable<IVMCollection> GetOwnerCollectionsOfVM(IViewModel vm) {
         return vm.Kernel.OwnerCollections
            .SelectMany(x => x.Cast<IVMCollection>())
            .ToArray();
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
         // bkauf owner refactoring
         //IVMCollection owner = item.Kernel.OwnerCollection;

         //if (owner != null) {
         //   var cache = owner.Behaviors.GetNextBehavior<CollectionValidationResultCacheBehavior>();
         //   IEnumerable<ValidationError> errors = cache.GetItemErrors(item, property);
         //   return new ValidationResult(errors);
         //}

         return ValidationResult.Valid;
      }
   }
}