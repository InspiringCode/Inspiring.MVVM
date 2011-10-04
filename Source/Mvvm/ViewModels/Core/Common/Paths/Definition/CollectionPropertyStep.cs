namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;

   internal sealed class CollectionPropertyStep<TDescriptor, TValue> :
      PathDefinitionStep
      where TDescriptor : IVMDescriptor {

      private PropertySelector<TDescriptor> _propertySelector;


      public CollectionPropertyStep(Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector) {
         Contract.Requires(propertySelector != null);
         _propertySelector = new PropertySelector<TDescriptor>(propertySelector);
      }

      public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
         if (!step.HasStep || !step.IsCollection) {
            return PathMatch.Fail();
         }

         IVMCollection parentCollection = step.Collection;
         step.MoveNext();

         if (!step.HasStep || !step.IsProperty) {
            return PathMatch.Fail();
         }

         if (!(parentCollection.GetItemDescriptor() is TDescriptor)) {
            return PathMatch.Fail();
         }

         var itemDescriptor = parentCollection.GetItemDescriptor();
         var expectedProperty = _propertySelector.GetProperty(itemDescriptor);

         if (expectedProperty == step.Property) {
            PathMatch result = PathMatch.Succeed(length: 1);
            PathMatch nextResult = definitionSteps.MatchesNext(step);

            return PathMatch.Combine(result, nextResult);
         }

         return PathMatch.Fail();
      }

      public override IViewModel[] GetDescendants(
         PathDefinitionIterator definitionSteps,
         IViewModel rootVM
      ) {
         throw new NotSupportedException();
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

         object expectedPropertyValue = parent.Kernel.GetValue(expectedProperty);

         if (nextStep.IsViewModel) {
            return nextStep.ViewModel == expectedPropertyValue;
         }

         if (nextStep.IsCollection) {
            return nextStep.Collection == expectedPropertyValue;
         }

         throw new NotSupportedException();
      }

      private bool Matches(IVMCollection collection, PathIterator nextStep) {
         bool canMatchAgainstItemDescriptor = nextStep.IsProperty;

         if (!canMatchAgainstItemDescriptor) {
            return false;
         }

         if (!(collection.GetItemDescriptor() is TDescriptor)) {
            return false;
         }

         TDescriptor itemDescriptor = (TDescriptor)collection.GetItemDescriptor();

         return _propertySelector.GetProperty(itemDescriptor) == nextStep.Property;
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
         return potentialItem.Kernel.OwnerCollections.Any(x => x.Equals(collection));
      }
   }
}
