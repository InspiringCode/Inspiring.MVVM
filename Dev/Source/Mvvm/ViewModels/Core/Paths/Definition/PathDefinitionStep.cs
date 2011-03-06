namespace Inspiring.Mvvm.ViewModels.Core {

   internal abstract class PathDefinitionStep {
      private PathDefinitionStep _next;

      public PathDefinitionStep Next {
         get { return _next ?? PathTerminationStep.Instance; }
         set { _next = value; }
      }

      public abstract PathMatch Matches(PathIterator path);
   }
}
