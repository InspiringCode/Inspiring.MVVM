﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class AnyPropertyStep<TDescriptor> :
      PathDefinitionStep
      where TDescriptor : VMDescriptorBase {

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

      public override string ToString() {
         return String.Format(
            "{0} -> {1}",
            TypeService.GetFriendlyName(typeof(TDescriptor)),
            "AnyProperty"
         );
      }

      public override IViewModel[] GetDescendants(PathDefinitionIterator definitionSteps, IViewModel rootVM) {
         throw new NotSupportedException();
      }
   }
}