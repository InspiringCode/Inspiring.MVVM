namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal abstract class PathDefinitionStep {
      private PathDefinitionStep _next;

      public PathDefinitionStep Next {
         get { return _next ?? PathTerminationStep.Instance; }
         set { _next = value; }
      }

      public abstract PathMatch Matches(PathIterator path);

      protected void ThrowUnexpectedStepTypeException(int index, params PathStepType[] expectedTypes) {
         throw new ArgumentException(
            ExceptionTexts.UnexpectedPathStepType.FormatWith(
               index,
               String.Join(", ", expectedTypes)
            )
         );
      }
   }
}
