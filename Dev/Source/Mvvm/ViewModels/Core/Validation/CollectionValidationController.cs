namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Linq;

   internal static class CollectionValidationController {
      public static CollectionResult Validate(ICollectionValidationTarget target) {
         ValidationResult collectionResult = target
            .Collection
            .OwnerVM
            .ExecuteValidationRequest(target.CreateValidationRequest());

         var possiblyStaleItems = GetPossiblyStaleItems(collectionResult, target);

         return new CollectionResult(target, collectionResult, possiblyStaleItems);
      }

      private static IEnumerable<IViewModel> GetPossiblyStaleItems(
         ValidationResult collectionResult,
         ICollectionValidationTarget target
      ) {
         // This method is performance critical! It is not a good idea to loop
         // through all items and call 'IsValid'. Remember that is method is called
         // for each item of every collection (because the framework tries to
         // perform a collection validation for every item).

         var allDescendantsErrors = target
            .Collection
            .OwnerVM
            .Kernel
            .GetValidationResult(ValidationResultScope.Descendants)
            .Errors;

         var previouslyInvalidItems = allDescendantsErrors
            .Where(e => e.OriginatedFrom(target))
            .Select(e => e.Target.VM);

         var failedItems = collectionResult
            .Errors
            .Select(x => x.Target.VM);

         return previouslyInvalidItems.Union(failedItems);
      }
   }
}