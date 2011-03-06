namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PathDefinition {
      //public static readonly PathDefinition Empty = new PathDefinition(new PathDefinitionStep[0]);
      //private readonly PathDefinitionStep[] _steps;

      private PathDefinitionStep _initialStep;
      private PathDefinitionStep _lastStep;

      private PathDefinition() {
         //_steps = steps;
      }

      public void Append(PathDefinitionStep step) {
         if (_initialStep == null) {
            _initialStep = step;
         } else {
            _lastStep.Next = step;
         }

         _lastStep = step;

         //var s = new PathDefinitionStep[_steps.Length + 1];
         //s[0] = step;
         //_steps.CopyTo(s, 1);

         //return new PathDefinition(s);
      }

      public PathMatch Matches(Path path) {
         return _initialStep.Matches(path.GetIterator());
      }
   }
}
