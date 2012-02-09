using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PathTerminationStep : PathDefinitionStep {
      public static readonly PathTerminationStep Instance = new PathTerminationStep();

      private PathTerminationStep() {
      }

      public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
         int matchedSteps = 0;

         if (step.HasStep) {
            step.MoveNext();
            matchedSteps++;
         }

         return step.HasStep ?
            PathMatch.Fail() :
            PathMatch.Succeed(length: matchedSteps);
      }

      public override IViewModel[] GetDescendants(
         PathDefinitionIterator definitionSteps,
         IViewModel rootVM, 
         bool onlyLoaded
      ) {
         return new IViewModel[] { rootVM };
      }

      public override string ToString(bool isFirst) {
         return String.Empty;
      }
   }
}
