namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class CollectionResult {
      private readonly ValidationResult _result;

      public CollectionResult(
         CollectionResultTarget target,
         ValidationResult result,
         IEnumerable<IViewModel> possiblyStaleItems
      ) {
         Target = target;
         _result = result;
         PossiblyStaleItems = possiblyStaleItems;
      }

      public CollectionResultTarget Target { get; private set; }
      public IEnumerable<IViewModel> PossiblyStaleItems { get; private set; }

      public ValidationResult GetResultFor(IViewModel item) {
         var itemErrors = _result
            .Errors
            .Where(x => x.Target == item);

         return new ValidationResult(itemErrors);
      }

      public override string ToString() {
         return String.Format(
            "{{ CollectionResult for {0}, Result = {1}, PossiblyStaleItems = [{2}] }}",
            Target,
            _result,
            String.Join(", ", PossiblyStaleItems)
         );
      }
   }
}
