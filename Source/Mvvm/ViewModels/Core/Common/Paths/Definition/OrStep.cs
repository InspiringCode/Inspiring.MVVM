namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   internal sealed class OrStep : PathDefinitionStep {
      private readonly PathDefinitionStep[] _steps;

      public OrStep(params PathDefinitionStep[] steps) {
         Check.NotEmpty(steps, nameof(steps));
         _steps = steps;
      }

      public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
         foreach (var orStep in _steps) {
            var match = orStep.Matches(definitionSteps, step);
            if (match.Success) {
               return match;
            }
         }
         return PathMatch.Fail();
      }

      public override IViewModel[] GetDescendants(PathDefinitionIterator definitionSteps, IViewModel rootVM, bool onlyLoaded) {
         throw new NotSupportedException();
      }

      public override string ToString(bool isFirst) {
         IEnumerable<PathDefinitionStep> steps = _steps;
         return String.Format(
            "[{0}]",
            String.Join(" OR ", steps)
         );
      }
   }
}