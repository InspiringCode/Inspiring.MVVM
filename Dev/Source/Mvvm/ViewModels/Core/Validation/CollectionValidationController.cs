namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Linq;

   internal static class CollectionValidationController {
      public static CollectionResult Validate(CollectionResultTarget target) {
         ValidationResult collectionResult = target
            .Collection
            .OwnerVM
            .ExecuteValidationRequest(target.CreateValidationRequest());

         var possiblyStaleItems = GetPossiblyStaleItems(collectionResult, target);

         return new CollectionResult(target, collectionResult, possiblyStaleItems);
      }

      private static IEnumerable<IViewModel> GetPossiblyStaleItems(
         ValidationResult collectionResult,
         CollectionResultTarget target
      ) {
         var previouslyInvalidItems = target
            .Collection
            .Cast<IViewModel>()
            .Where(item => IsInvalid(item, target.Property));

         var failedItems = collectionResult
            .Errors
            .Select(x => x.Target);

         return previouslyInvalidItems.Union(failedItems);
      }

      private static bool IsInvalid(
         IViewModel item,
         IVMPropertyDescriptor property = null
      ) {
         return item
            .Kernel
            .GetValidationResult(ValidationResultScope.Self)
            .Errors
            .Any(x => x.TargetProperty == property);
      }
   }
}
