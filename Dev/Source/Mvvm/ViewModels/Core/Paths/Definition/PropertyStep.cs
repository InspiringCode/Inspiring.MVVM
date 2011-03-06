namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class PropertyStep<TDescriptor> :
      PathDefinitionStep
      where TDescriptor : VMDescriptorBase {

      private Func<TDescriptor, IVMPropertyDescriptor> _propertySelector;

      public PropertyStep(Func<TDescriptor, IVMPropertyDescriptor> propertySelector) {
         Contract.Requires(propertySelector != null);
         _propertySelector = propertySelector;
      }

      public override PathMatch Matches(PathIterator step) {
         if (!step.HasStep) {
            return PathMatch.Fail();
         }

         if (!step.IsViewModel) {
            ThrowUnexpectedStepTypeException(step.GetIndex(), PathStepType.ViewModel);
         }

         IViewModel parent = step.ViewModel;
         step.MoveNext();

         if (Matches(parent, step)) {
            int matchedPathSteps = 1;

            if (CollectionIsFollowedByItemViewModel(step)) {
               step.MoveNext();
               matchedPathSteps++;
            }

            PathMatch result = PathMatch.Succeed(length: matchedPathSteps);
            PathMatch nextResult = Next.Matches(step);

            return PathMatch.Combine(result, nextResult);
         } else {
            return PathMatch.Fail();
         }
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
            return nextStep.ViewModel == expectedPropertyValue;
         }

         if (nextStep.IsCollection) {
            return nextStep.Collection == expectedPropertyValue;
         }

         throw new NotSupportedException();
      }

      private static bool CollectionIsFollowedByItemViewModel(PathIterator step) {
         if (!step.IsCollection) {
            return false;
         }

         IVMCollection collection = step.Collection;

         step.MoveNext();

         if (!step.HasStep || !step.IsViewModel) {
            return false;
         }

         IViewModel potentialItem = step.ViewModel;
         return potentialItem.Kernel.OwnerCollection == collection;
      }
   }
}
