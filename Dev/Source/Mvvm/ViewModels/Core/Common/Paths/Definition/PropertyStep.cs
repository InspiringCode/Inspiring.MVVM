namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   internal sealed class PropertyStep<TDescriptor, TValue> :
      PathDefinitionStep
      where TDescriptor : VMDescriptorBase {

      private Func<TDescriptor, IVMPropertyDescriptor<TValue>> _propertySelector;

      public PropertyStep(Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector) {
         Contract.Requires(propertySelector != null);
         _propertySelector = propertySelector;
      }

      public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
         if (!step.HasStep || step.IsCollection) {
            return PathMatch.Fail();
         }

         if (step.IsProperty) {
            ThrowUnexpectedStepTypeException(step.GetIndex(), PathStepType.ViewModel, PathStepType.Collection);
         }

         PathIterator parentStep = step;
         step.MoveNext();

         if (!step.HasStep) {
            return PathMatch.Fail();
         }

         bool currenStepMatches = Matches(parentStep.ViewModel, step);

         if (currenStepMatches) {
            PathMatch result = PathMatch.Succeed(length: 1);
            PathMatch nextResult = definitionSteps.MatchesNext(step);

            return PathMatch.Combine(result, nextResult);
         } else {
            return PathMatch.Fail();
         }
      }

      public override string ToString() {
         return String.Format(
            "{0} -> {1}",
            TypeService.GetFriendlyName(typeof(TDescriptor)),
            TypeService.GetFriendlyName(typeof(TValue))
         );
      }

      private bool Matches(IViewModel parent, PathIterator nextStep) {
         if (!nextStep.HasStep) {
            return false;
         }

         TDescriptor descriptor = parent.Descriptor as TDescriptor;

         if (descriptor == null) {
            return false;
         }

         IVMPropertyDescriptor expectedProperty = _propertySelector(descriptor);

         if (nextStep.IsProperty) {
            return nextStep.Property == expectedProperty;
         }

         object expectedPropertyValue = parent.Kernel.GetValue(expectedProperty);

         if (nextStep.IsViewModel) {
            if (nextStep.ViewModel == expectedPropertyValue) {
               return true;
            } else {
               if (expectedPropertyValue is IVMCollection) {
                  var parentCollection = (IVMCollection)expectedPropertyValue;

                  return nextStep
                     .ViewModel
                     .Kernel
                     .OwnerCollections
                     .Contains(parentCollection);
               } else {
                  return false;
               }
            }
         }

         if (nextStep.IsCollection) {
            return nextStep.Collection == expectedPropertyValue;
         }

         throw new NotSupportedException();
      }
   }
}
