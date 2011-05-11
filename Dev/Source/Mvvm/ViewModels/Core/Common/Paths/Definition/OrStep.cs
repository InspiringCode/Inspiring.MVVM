namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   internal sealed class OrStep : PathDefinitionStep {
      private readonly PathDefinitionStep[] _steps;

      public OrStep(params PathDefinitionStep[] steps) {
         Contract.Requires<ArgumentException>(steps.Count() > 0);
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

      public override IViewModel[] GetDescendants(PathDefinitionIterator definitionSteps, IViewModel rootVM) {
         throw new NotSupportedException();
      }

      public override string ToString() {
         IEnumerable<PathDefinitionStep> steps = _steps;
         return String.Format(
            "OrStep[{0}]",
             String.Join(", ", steps)
         );
      }
   }
}