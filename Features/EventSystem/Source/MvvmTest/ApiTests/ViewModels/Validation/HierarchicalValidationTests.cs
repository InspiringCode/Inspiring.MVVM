namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels;

   [TestClass]
   public class HierarchicalValidationTests : HierarchicalValidationFixture {
      [TestMethod]
      public void Revalidate_WhenDescendantAddsValidationError_AddsErrorToAncestorValidationResult() {
         Results.SetupFailing().PropertyValidation
            .Targeting(Child, x => x.ChildProperty)
            .On(Child);

         Results.SetupFailing().ViewModelValidation
            .Targeting(Child)
            .On(Child);

         Child.Revalidate(x => x.ChildProperty);

         Results.VerifySetupValidationResults();

         var expectedAncestorResult = new ValidationResult(
            Results.ValidatorSetups.SelectMany(x => x.Result.Errors)
         );

         ValidationAssert.AreEqual(expectedAncestorResult, Grandparent.ValidationResult);
      }

      [TestMethod]
      public void Revalidate_WhenDescendantAddsCollectionValidationError_AddsErrorToAncestorValidationResult() {
         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(Item, x => x.ItemProperty)
            .On(Child);

         Results.SetupFailing().CollectionViewModelValidation
            .Targeting(Item)
            .On(Child);

         Item.Revalidate(x => x.ItemProperty);

         Results.VerifySetupValidationResults();

         var expectedAncestorResult = new ValidationResult(
            Results.ValidatorSetups.SelectMany(x => x.Result.Errors)
         );

         ValidationAssert.AreEqual(expectedAncestorResult, Grandparent.ValidationResult);
      }

      [TestMethod]
      public void Revalidate_ExecutesAncestorPropertyValidators() {
         Results.SetupFailing().PropertyValidation
            .Targeting(Child, x => x.ChildProperty)
            .On(Child, Parent, Grandparent);

         Results.ExpectInvocationOf.PropertyValidation
            .Targeting(Child, x => x.ChildProperty)
            .On(Child, Parent, Grandparent);

         ExpectInvocationOfViewModelValidatorsFromChildToGrandparentBecauseValidationStateChanged();

         Child.Revalidate(x => x.ChildProperty);
         Results.VerifySequenceAndResults();
      }


      [TestMethod]
      public void Revalidate_ExecutesAncestorViewModelValidators() {
         Results.SetupFailing().ViewModelValidation
            .Targeting(Child)
            .On(Child, Parent, Grandparent);

         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Child)
            .On(Child, Parent, Grandparent);

         ExpectInvocationOfViewModelValidatorsFromChildToGrandparentBecauseValidationStateChanged();

         Child.RevalidateViewModelValidations();
         Results.VerifySequenceAndResults();
      }

      [TestMethod]
      public void Revalidate_ExecutesAncestorCollectionPropertyValidators() {
         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(Item, x => x.ItemProperty)
            .On(Child, Parent, Grandparent);

         Results.ExpectInvocationOf.PropertyValidation
            .Targeting(Item, x => x.ItemProperty)
            .On(Item, Child, Parent, Grandparent);

         Results.ExpectInvocationOf.CollectionPropertyValidation
            .Targeting(Child.Items, x => x.ItemProperty)
            .On(Child, Parent, Grandparent);

         ExpectInvocationOfViewModelValidatorsFromItemToGrandparentBecauseValidationStateChanged();

         Item.Revalidate(x => x.ItemProperty);
         Results.VerifySequenceAndResults();
      }

      [TestMethod]
      public void Revalidate_ExecutesAncestorCollectionViewModelValidators() {
         Results.SetupFailing().CollectionViewModelValidation
            .Targeting(Item)
            .On(Child, Parent, Grandparent);

         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Item)
            .On(Item, Child, Parent, Grandparent);

         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(Child.Items)
            .On(Child, Parent, Grandparent);

         ExpectInvocationOfViewModelValidatorsFromItemToGrandparentBecauseValidationStateChanged();

         Item.RevalidateViewModelValidations();
         Results.VerifySequenceAndResults();
      }

      private void ExpectInvocationOfViewModelValidatorsFromChildToGrandparentBecauseValidationStateChanged() {
         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Child)
            .On(Child, Parent, Grandparent);

         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Parent)
            .On(Parent, Grandparent);

         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Grandparent)
            .On(Grandparent);
      }

      private void ExpectInvocationOfViewModelValidatorsFromItemToGrandparentBecauseValidationStateChanged() {
         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Item)
            .On(Item, Child, Parent, Grandparent);

         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(Child.Items)
            .On(Child, Parent, Grandparent);

         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Child)
            .On(Child, Parent, Grandparent);

         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Parent)
            .On(Parent, Grandparent);

         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Grandparent)
            .On(Grandparent);
      }
   }
}