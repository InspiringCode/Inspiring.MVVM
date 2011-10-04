namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class OptionalStep : PathDefinitionStep {
      private readonly PathDefinitionStep _innerStep;

      public OptionalStep(PathDefinitionStep innerStep) {
         _innerStep = innerStep;
      }


      public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
         var match = _innerStep.Matches(definitionSteps, step);
         if (match.Success) {
            return match;
         } else {
            return definitionSteps.MatchesNext(step);
         }
      }

      public override IViewModel[] GetDescendants(PathDefinitionIterator definitionSteps, IViewModel rootVM) {
         throw new NotSupportedException();
      }

      public override string ToString(bool isFirst) {
         return String.Format("{0}?", _innerStep.ToString(isFirst));
      }
   }
}
