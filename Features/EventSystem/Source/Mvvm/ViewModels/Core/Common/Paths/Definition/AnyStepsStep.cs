namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class AnyStepsStep<TDescriptor> :
      PathDefinitionStep
      where TDescriptor : IVMDescriptor {

      public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
         if (!step.HasStep || step.IsCollection || step.IsProperty) {
            return PathMatch.Fail();
         }
         int pathLength = 0;
         PathIterator parentStep = step;
         step.MoveNext();
         pathLength++;

         bool currentStepMatches = Matches(parentStep.ViewModel, step);

         if (currentStepMatches) {
            while (step.HasStep) {
               step.MoveNext();
               pathLength++;
            }

            return PathMatch.Succeed(length: pathLength);
         } else {
            return PathMatch.Fail();
         }
      }

      public override IViewModel[] GetDescendants(PathDefinitionIterator definitionSteps, IViewModel rootVM) {
         throw new System.NotImplementedException();
      }

      public override string ToString() {
         return String.Format(
            "{0} -> {1}",
            TypeService.GetFriendlyName(typeof(TDescriptor)),
            "AnySteps"
         );
      }

      private bool Matches(IViewModel parent, PathIterator nextStep) {
         if (!nextStep.HasStep || !(parent.Descriptor is TDescriptor)) {
            return false;
         }
         if (nextStep.Property != null && !parent.Descriptor.Properties.Contains(nextStep.Property)) {
            throw new ArgumentException(ExceptionTexts.PropertyIsNotContainedByParentDescriptor);
         }
         return true;
      }
   }
}
