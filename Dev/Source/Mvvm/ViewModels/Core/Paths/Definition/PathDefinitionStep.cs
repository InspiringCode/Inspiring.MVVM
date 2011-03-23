namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal abstract class PathDefinitionStep {
      public abstract PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step);

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
