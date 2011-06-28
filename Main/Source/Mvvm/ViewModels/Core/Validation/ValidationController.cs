namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   public sealed class ValidationController {
      private readonly List<CollectionResult> _cachedCollectionResults = new List<CollectionResult>();
      private readonly ValidationQueue _validationQueue = new ValidationQueue();
      private readonly List<RevalidationRequest> _currentlyValidating = new List<RevalidationRequest>();

      public void RequestPropertyRevalidation(
         IViewModel target,
         IVMPropertyDescriptor targetProperty
      ) {
         Contract.Requires(target != null);
         Contract.Requires(targetProperty != null);

         EnqueueRevalidation(new RevalidationRequest(target, targetProperty));
      }

      public void RequestViewModelRevalidation(IViewModel target) {
         Contract.Requires(target != null);
         EnqueueRevalidation(new RevalidationRequest(target));
      }

      public void ProcessPendingValidations() {
         while (_validationQueue.HasRequests) {
            // We dequeue the request after the revalidation to avoid that the same
            // request that is currently processed is added to the queue again.

            RevalidationRequest r = _validationQueue.Peek();
            r.PerformRevalidation(this);

            _validationQueue.Dequeue();
         }
      }

      public void ManuallyBeginValidation(
         IViewModel target,
         IVMPropertyDescriptor targetProperty
      ) {
         var equivalentRequest = new RevalidationRequest(target, targetProperty);
         _currentlyValidating.Add(equivalentRequest);

         IBehaviorContext context = target.Kernel.GetContext();
         targetProperty.Behaviors.BeginValidationNext(context, this);
      }

      public void ManuallyEndValidation(
         IViewModel target,
         IVMPropertyDescriptor targetProperty
      ) {
         var equivalentRequest = new RevalidationRequest(target, targetProperty);
         _currentlyValidating.Remove(equivalentRequest);

         IBehaviorContext context = target.Kernel.GetContext();
         targetProperty.Behaviors.EndValidationNext(context);
      }

      public void InvalidCollectionResults(
         ValidationStep step,
         IViewModel item,
         IVMPropertyDescriptor property
      ) {
         _cachedCollectionResults.RemoveAll(res =>
            res.Target.Step == step &&
            res.Target.Property == property &&
            item.Kernel.OwnerCollections.Contains(res.Target.Collection)
         );
      }

      public ValidationResult GetResult(
         ValidationStep step,
         IViewModel target,
         IVMPropertyDescriptor targetProperty = null
      ) {
         var t = ValidationTarget.ForInstance(step, target, targetProperty);

         var request = t.CreateValidationRequest();
         var result = t.VM.ExecuteValidationRequest(request);

         var collectionResult = GetCollectionItemResults(t);

         return ValidationResult.Join(result, collectionResult);
      }

      private ValidationResult GetCollectionItemResults(IInstanceValidationTarget item) {
         var result = ValidationResult.Valid;

         foreach (var ownerCollection in item.VM.Kernel.OwnerCollections) {
            CollectionResult collectionResult = ValidateCollectionIfNotInCache(
               ValidationTarget.ForCollection(
                  item.Step,
                  ownerCollection,
                  item.Property
               )
            );

            var itemResult = collectionResult.GetResultFor(item.VM);
            result = ValidationResult.Join(result, itemResult);
         }

         return result;
      }

      private CollectionResult ValidateCollectionIfNotInCache(ICollectionValidationTarget target) {
         CollectionResult cachedResult = _cachedCollectionResults
            .FirstOrDefault(cached => target.Equals(cached.Target));

         if (cachedResult == null) {
            cachedResult = CollectionValidationController.Validate(target);
            _cachedCollectionResults.Add(cachedResult);

            foreach (IViewModel item in cachedResult.PossiblyStaleItems) {
               var r = new RevalidationRequest(item, target.Property);
               EnqueueRevalidation(r);
            }
         }

         return cachedResult;
      }

      private void EnqueueRevalidation(RevalidationRequest target) {
         if (!_currentlyValidating.Contains(target)) {
            _validationQueue.Enqueue(target);
         }
      }

      private class RevalidationRequest {
         public RevalidationRequest(IViewModel vm, IVMPropertyDescriptor property = null) {
            VM = vm;
            Property = property;
         }

         public IViewModel VM { get; private set; }
         public IVMPropertyDescriptor Property { get; private set; }

         public void PerformRevalidation(ValidationController controller) {
            IBehaviorContext context = VM.Kernel.GetContext();

            bool targetsProperty = Property != null;
            if (targetsProperty) {
               Property.Behaviors.BeginValidationNext(context, controller);
               Property.Behaviors.PropertyRevalidateNext(context);
               Property.Behaviors.EndValidationNext(context);
            }

            bool targetsViewModel = Property == null;
            if (targetsViewModel) {
               VM.Descriptor.Behaviors.ViewModelRevalidateNext(context, controller);
            }
         }

         public override bool Equals(object obj) {
            var other = obj as RevalidationRequest;
            return other != null && other.VM == VM && other.Property == Property;
         }

         public override int GetHashCode() {
            return HashCodeService.CalculateHashCode(this, VM, Property);
         }

         public override string ToString() {
            return Property != null ?
               String.Format("{{RevalidationRequest for {0}.{1}}}", VM, Property) :
               String.Format("{{RevalidationRequest for {0}}}", VM);
         }
      }

      private class ValidationQueue {
         private HashSet<RevalidationRequest> _set = new HashSet<RevalidationRequest>();
         private Queue<RevalidationRequest> _queue = new Queue<RevalidationRequest>();

         public bool HasRequests {
            get { return _queue.Count > 0; }
         }

         public void Enqueue(RevalidationRequest request) {
            if (_set.Contains(request)) {
               return;
            }

            _set.Add(request);
            _queue.Enqueue(request);
         }

         public RevalidationRequest Peek() {
            return _queue.Peek();
         }

         public void Dequeue() {
            var r = _queue.Dequeue();
            _set.Remove(r);
         }
      }
   }
}
