namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class CollectionStep<TDescriptor, TItemVM> :
      PathDefinitionStep
      where TDescriptor : VMDescriptorBase
      where TItemVM : IViewModel {

      private Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<TItemVM>>> _propertySelector;

      public CollectionStep(Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<TItemVM>>> propertySelector) {
         Contract.Requires(propertySelector != null);
         _propertySelector = propertySelector;
      }

      public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
         if (!step.HasStep || !step.IsViewModel) {
            return PathMatch.Fail();
         }

         IViewModel parentViewModel = step.ViewModel;
         step.MoveNext();

         if (!step.HasStep || !step.IsCollection) {
            return PathMatch.Fail();
         }

         var descriptor = parentViewModel.Descriptor as TDescriptor;

         if (descriptor == null) {
            return PathMatch.Fail();
         }

         var collectionProperty = _propertySelector(descriptor);
         var expectedCollection = parentViewModel.Kernel.GetValue((IVMPropertyDescriptor)collectionProperty); // HACK ATTACK

         if (step.Collection == expectedCollection) {
            PathMatch result = PathMatch.Succeed(length: 1);
            PathMatch nextResult = definitionSteps.MatchesNext(step);

            return PathMatch.Combine(result, nextResult);
         } else {
            return PathMatch.Fail();
         }

         //var itemDescriptor = parentCollection.GetItemDescriptor() as TDescriptor;

         //if (itemDescriptor == null) {
         //   return PathMatch.Fail();
         //}

         //var expectedProperty = _propertySelector(itemDescriptor);

         //if (expectedProperty == step.Property) {
         //   PathMatch result = PathMatch.Succeed(length: 1);
         //   PathMatch nextResult = definitionSteps.MatchesNext(step);

         //   return PathMatch.Combine(result, nextResult);
         //}

         //return PathMatch.Fail();

         //if (!step.HasStep) {
         //   return PathMatch.Fail();
         //}

         //if (!step.IsViewModel && !step.IsCollection) {
         //   ThrowUnexpectedStepTypeException(step.GetIndex(), PathStepType.ViewModel, PathStepType.Collection);
         //}

         //PathIterator parentStep = step;
         //step.MoveNext();

         //if (!step.HasStep) {
         //   return PathMatch.Fail();
         //}

         //bool currenStepMatches = false;

         //switch (parentStep.Type) {
         //   case PathStepType.ViewModel:
         //      currenStepMatches = Matches(parentStep.ViewModel, step);
         //      break;
         //   case PathStepType.Collection:
         //      currenStepMatches = Matches(parentStep.Collection, step);
         //      break;
         //}

         //if (currenStepMatches) {
         //   int matchedPathSteps = 1;

         //   if (CollectionIsFollowedByItemViewModel(step)) {
         //      step.MoveNext();
         //      matchedPathSteps++;
         //   }

         //   PathMatch result = PathMatch.Succeed(length: matchedPathSteps);
         //   PathMatch nextResult = definitionSteps.MatchesNext(step);

         //   return PathMatch.Combine(result, nextResult);
         //} else {
         //   return PathMatch.Fail();
         //}
      }

      public override string ToString() {
         return String.Format(
            "{0} -> {1}",
            TypeService.GetFriendlyName(typeof(TDescriptor)),
            TypeService.GetFriendlyName(typeof(IVMCollection<TItemVM>))
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

         TDescriptor itemDescriptor = collection.GetItemDescriptor() as TDescriptor;

         if (itemDescriptor == null) {
            return false;
         }

         return _propertySelector(itemDescriptor) == nextStep.Property;
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
