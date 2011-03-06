namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PathDefinition {
      public static readonly PathDefinition Empty = new PathDefinition(new PathDefinitionStep[0]);

      private readonly PathDefinitionStep[] _steps;

      private PathDefinition(PathDefinitionStep[] steps) {
         _steps = steps;
      }

      public PathDefinition Append(PathDefinitionStep step) {
         var s = new PathDefinitionStep[_steps.Length + 1];
         s[0] = step;
         _steps.CopyTo(s, 1);

         return new PathDefinition(s);
      }
   }
}
