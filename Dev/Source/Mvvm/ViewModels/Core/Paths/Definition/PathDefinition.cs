namespace Inspiring.Mvvm.ViewModels.Core {
   internal sealed class PathDefinition {
      public static readonly PathDefinition Empty = new PathDefinition(new PathDefinitionStep[0]);

      private readonly PathDefinitionStep[] _steps;

      private PathDefinition(PathDefinitionStep[] steps) {
         _steps = steps;
      }

      public PathDefinition Append(PathDefinitionStep step) {
         return new PathDefinition(ArrayUtils.Append(_steps, step));
      }

      public PathMatch Matches(Path path) {
         var it = new PathDefinitionIterator(_steps);
         return it.MatchesNext(path.GetIterator());
      }
   }
}
