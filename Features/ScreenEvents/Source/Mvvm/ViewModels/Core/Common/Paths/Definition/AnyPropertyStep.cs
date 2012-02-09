namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class AnyPropertyStep<TDescriptor> :
      PathDefinitionStep
      where TDescriptor : IVMDescriptor {

      public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
         if (!step.HasStep || step.IsCollection || step.IsProperty) {
            return PathMatch.Fail();
         }

         PathIterator parentStep = step;
         step.MoveNext();

         bool currentStepMatches = Matches(parentStep.ViewModel, step);

         if (currentStepMatches) {
            PathMatch result = PathMatch.Succeed(length: 1);
            PathMatch nextResult = definitionSteps.MatchesNext(step);

            return PathMatch.Combine(result, nextResult);
         } else {
            return PathMatch.Fail();
         }
      }

      private bool Matches(IViewModel parent, PathIterator nextStep) {
         if (!nextStep.HasStep) {
            return false;
         }
         if (!(parent.Descriptor is TDescriptor)) {
            return false;
         }
         if (!nextStep.IsProperty) {
            return false;
         }
         if (!parent.Descriptor.Properties.Contains(nextStep.Property)) {
            throw new ArgumentException(ExceptionTexts.PropertyIsNotContainedByParentDescriptor);
         }
         return true;
      }

      public override string ToString(bool isFirst) {
         if (!isFirst) {
            return "[any property]";
         }

         return String.Format(
            "{0}.[any property]",
            TypeService.GetFriendlyName(typeof(TDescriptor))
         );
      }

      public override IViewModel[] GetDescendants(PathDefinitionIterator definitionSteps, IViewModel rootVM, bool onlyLoaded) {
         throw new NotSupportedException();
      }
   }
}