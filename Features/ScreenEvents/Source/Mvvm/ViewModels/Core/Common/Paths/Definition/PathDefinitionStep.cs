namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal abstract class PathDefinitionStep {
      public abstract PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step);

      public abstract IViewModel[] GetDescendants(PathDefinitionIterator definitionSteps, IViewModel rootVM, bool onlyLoaded);

      protected void ThrowUnexpectedStepTypeException(int index, params PathStepType[] expectedTypes) {
         throw new ArgumentException(
            ExceptionTexts.UnexpectedPathStepType.FormatWith(
               index,
               String.Join(", ", expectedTypes)
            )
         );
      }

      public abstract string ToString(bool isFirst);

      public sealed override string ToString() {
         return ToString(isFirst: true);
      }
   }
}
