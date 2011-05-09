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
         var slowPreviouslyInvalidItems = target
            .Collection
            .Cast<IViewModel>()
            .Where(item => IsInvalid(item, target.Property));

         var allDescendantsErrors = target
            .Collection
            .OwnerVM
            .Kernel
            .GetValidationResult(ValidationResultScope.Descendants)
            .Errors;

         var fastPreviouslyInvalidItems = allDescendantsErrors
            .Where(e => e.OriginatedFrom(target))
            .Select(e => e.Target.VM);

         var tooLess = slowPreviouslyInvalidItems.Except(fastPreviouslyInvalidItems).ToArray();
         var tooMuch = fastPreviouslyInvalidItems.Except(slowPreviouslyInvalidItems).ToArray();

         //Contract.Assert(!tooLess.Any() && !tooMuch.Any());

         var failedItems = collectionResult
            .Errors
            .Select(x => x.Target.VM);

         return slowPreviouslyInvalidItems.Union(failedItems);
      }

      private static bool IsInvalid(
         IViewModel item,
         IVMPropertyDescriptor property = null
      ) {
         return item
            .Kernel
            .GetValidationResult(ValidationResultScope.Self)
            .Errors
            .Any(x => x.Target.Property == property);
      }
   }
}
