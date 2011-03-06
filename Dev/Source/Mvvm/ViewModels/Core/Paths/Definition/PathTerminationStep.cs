namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PathTerminationStep : PathDefinitionStep {
      public static readonly PathTerminationStep Instance = new PathTerminationStep();

      private PathTerminationStep() {
      }

      public override PathMatch Matches(PathIterator path) {
         int matchedSteps = 0;

         if (path.HasStep) {
            path.MoveNext();
            matchedSteps++;
         }

         return path.HasStep ?
            PathMatch.Fail() :
            PathMatch.Succeed(length: matchedSteps);
      }
   }
}
