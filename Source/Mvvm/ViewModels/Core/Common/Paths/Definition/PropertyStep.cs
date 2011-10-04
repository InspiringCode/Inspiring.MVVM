namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class PropertyStep<TDescriptor> :
      PathDefinitionStep
      where TDescriptor : IVMDescriptor {

      private readonly PropertySelector<TDescriptor> _propertySelector;

      public PropertyStep(
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) {
         Contract.Requires(propertySelector != null);
         _propertySelector = new PropertySelector<TDescriptor>(propertySelector);
      }

      //public PropertyStep(
      //   Func<TDescriptor, IVMPropertyDescriptor> propertySelector,
      //   Type propertyHint
      //) {
      //   Contract.Requires(propertySelector != null);
      //   _propertySelector = propertySelector;
      //   _propertyNameHint = String.Format("IVMPropertyDescriptor<{0}>", TypeService.GetFriendlyName(propertyHint));
      //}

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
         var property = _propertySelector.GetProperty(rootVM.Descriptor);
         var instance = rootVM.Kernel.GetValue(property);

         if (PropertyTypeHelper.IsViewModelCollection(property.PropertyType)) {
            if (instance == null) {
               return new IViewModel[0];
            }

            var collection = (IVMCollection)instance;
            return ((IVMCollection)instance)
               .Cast<IViewModel>()
               .SelectMany(x => definitionSteps.GetDescendantNext(x))
               .ToArray();

         } else if (PropertyTypeHelper.IsViewModel(property.PropertyType)) {
            if (instance == null) {
               return new IViewModel[0];
            }

            return definitionSteps.GetDescendantNext((IViewModel)instance);
         }

         throw new NotSupportedException(ExceptionTexts.GetDescaedantsWrongPropertyStepType);
      }

      public override string ToString(bool isFirst) {
         return isFirst ?
            _propertySelector.ToString() :
            _propertySelector.PropertyName;
      }

      private bool Matches(IViewModel parent, PathIterator nextStep) {
         if (!nextStep.HasStep) {
            return false;
         }

         if (!(parent.Descriptor is TDescriptor)) {
            return false;
         }

         TDescriptor descriptor = (TDescriptor)parent.Descriptor;

         IVMPropertyDescriptor expectedProperty = _propertySelector.GetProperty(descriptor);

         if (nextStep.IsProperty) {
            return nextStep.Property == expectedProperty;
         }

         if (!parent.Kernel.IsLoaded(expectedProperty)) {
            return false;
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