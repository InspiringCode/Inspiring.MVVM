﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class CollectionResultCache {
      private List<KeyValuePair<CollectionResultKey, ValidationResult>> _cache =
         new List<KeyValuePair<CollectionResultKey, ValidationResult>>();

      public CollectionResultCache() {
         ValidatedCollections = new List<IVMCollection>();
      }

      private List<IVMCollection> ValidatedCollections { get; set; }

      public ValidationResult GetCollectionValidationResults(
         ValidationStep step,
         IViewModel item,
         IVMPropertyDescriptor property
      ) {
         var builder = new CollectionValidationController(this, step, property);
         
         return ValidationResult.Join(
            item.Kernel.OwnerCollections.Select(collection => {
               builder.EnsureCollectionWasValidated(collection);
               return FindResult(collection);
            })
         );
      }

      private ValidationResult FindResult(IVMCollection collection) {
         var entry = _cache.Single(x => x.Key.Collection == collection);
         return entry.Value;
      }
      
      private void Add(CollectionResultKey key, ValidationResult result) {
         _cache.Add(new KeyValuePair<CollectionResultKey, ValidationResult>(key, result));
      }

      private class CollectionResultKey {
         public CollectionResultKey(
            ValidationStep step,
            IVMCollection collection,
            IVMPropertyDescriptor property
         ) {
            Step = step;
            Collection = collection;
            Property = property;
         }

         public ValidationStep Step { get; private set; }
         public IVMCollection Collection { get; private set; }
         public IVMPropertyDescriptor Property { get; private set; }
      }

      private class CollectionValidationController {
         private readonly CollectionResultCache _cache;
         private readonly ValidationStep _step;
         private readonly IVMPropertyDescriptor _property;
         private readonly Queue<IVMCollection> _unvalidatedQueue;

         public CollectionValidationController(CollectionResultCache cache, ValidationStep step, IVMPropertyDescriptor property) {
            _cache = cache;
            _step = step;
            _property = property;
            _unvalidatedQueue = new Queue<IVMCollection>();
         }

         public void EnsureCollectionWasValidated(IVMCollection collection) {
            if (!_cache.ValidatedCollections.Contains(collection)) {
               PerformCollectionValidation(collection);
            }
         }

         private void PerformCollectionValidation(IVMCollection collection) {
            _unvalidatedQueue.Enqueue(collection);

            while (_unvalidatedQueue.Any()) {
               var c = _unvalidatedQueue.Dequeue();

               var key = new CollectionResultKey(_step, c, _property);
               var result = InvokeCollectionValidationExecutors(key);
               _cache.Add(key, result);

               var staleItems = GetPossiblyStaleItems(c, result);
               QueueUnvalidatedOwnerCollections(staleItems);
            }
         }

         private ValidationResult InvokeCollectionValidationExecutors(CollectionResultKey key) {
            var requestPath = Path.Empty
               .Append(key.Collection.Owner)
               .Append(key.Collection);

            if (key.Property != null) {
               requestPath = requestPath.Append(key.Property);
            }

            var request = new ValidationRequest(key.Step, requestPath);
            return key.Collection.Owner.ExecuteValidationRequest(request);
         }

         private IEnumerable<IViewModel> GetPossiblyStaleItems(
            IVMCollection collection,
            ValidationResult collectionResult
         ) {
            var previouslyInvalidItems = collection
               .Cast<IViewModel>()
               .Where(PropertyIsValid);

            var failedItems = collectionResult.Errors.Select(x => x.Target);

            return previouslyInvalidItems.Union(failedItems);
         }

         private bool PropertyIsValid(IViewModel viewModel) {
            return viewModel
               .Kernel
               .GetValidationState(ValidationStateScope.Self)
               .Errors
               .Any(x => x.TargetProperty == _property);
         }

         private void QueueUnvalidatedOwnerCollections(IEnumerable<IViewModel> items) {
            var validatedOrQueued = _cache.ValidatedCollections.Concat(_unvalidatedQueue);

            items
               .SelectMany(x => x.Kernel.OwnerCollections)
               .Except(validatedOrQueued)
               .ForEach(_unvalidatedQueue.Enqueue);
         }
      }
   }
}