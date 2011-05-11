namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class PropertyStep<TDescriptor> :
      PathDefinitionStep
      where TDescriptor : IVMDescriptor {

      private readonly Func<TDescriptor, IVMPropertyDescriptor> _propertySelector;
      private readonly string _propertyNameHint;

      public PropertyStep(
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) {
         Contract.Requires(propertySelector != null);
         _propertySelector = propertySelector;
         _propertyNameHint = "IVMPropertyDescriptor";
      }

      public PropertyStep(
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector,
         Type propertyHint
      ) {
         Contract.Requires(propertySelector != null);
         _propertySelector = propertySelector;
         _propertyNameHint = String.Format("IVMPropertyDescriptor<{0}>", TypeService.GetFriendlyName(propertyHint));
      }

      public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
         if (!step.HasStep || step.IsCollection || step.IsProperty) {
            return PathMatch.Fail();
         }

         PathIterator parentStep = step;
         step.MoveNext();

         if (!step.HasStep) {
            return PathMatch.Fail();
         }

         bool currentStepMatches = Matches(parentStep.ViewModel, step);

         if (currentStepMatches) {
            PathMatch result = PathMatch.Succeed(length: 1);
            PathMatch nextResult = definitionSteps.MatchesNext(step);

            return PathMatch.Combine(result, nextResult);
         } else {
            return PathMatch.Fail();
         }
      }

      public override IViewModel[] GetDescendants(
         PathDefinitionIterator definitionSteps,
         IViewModel rootVM
      ) {
         var property = _propertySelector((TDescriptor)rootVM.Descriptor);
         var instance = rootVM.Kernel.GetValue(property);

         if (instance is IVMCollection) {
            var collection = (IVMCollection)instance;
            return ((IVMCollection)instance)
               .Cast<IViewModel>()
               .SelectMany(x => definitionSteps.GetDescendantNext(x))
               .ToArray();

         } else if (instance is IViewModel) {
            return definitionSteps.GetDescendantNext((IViewModel)instance);
         }

         throw new NotSupportedException(ExceptionTexts.GetDescaedantsWrongPropertyStepType);
      }

      public override string ToString() {
         return String.Format(
            "{0} -> {1}",
            TypeService.GetFriendlyName(typeof(TDescriptor)),
            _propertyNameHint
         );
      }

      private bool Matches(IViewModel parent, PathIterator nextStep) {
         if (!nextStep.HasStep) {
            return false;
         }

         if (!(parent.Descriptor is TDescriptor)) {
            return false;
         }

         TDescriptor descriptor = (TDescriptor)parent.Descriptor;

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
            return false;
         }

         throw new NotSupportedException();
      }
   }
}