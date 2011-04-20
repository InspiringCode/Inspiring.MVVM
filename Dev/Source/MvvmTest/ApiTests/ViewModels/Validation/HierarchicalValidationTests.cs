namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class HierarchicalValidationTests : HierarchicalValidationFixture {
      [TestMethod]
      public void Revalidate_ExecutesAncestorPropertyValidators() {
         Results.SetupFailing.PropertyValidation
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
         Results.SetupFailing.ViewModelValidation
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
         Results.SetupFailing.CollectionPropertyValidation
            .Targeting(Item, x => x.ItemProperty)
            .On(Child, Parent, Grandparent);

         Results.ExpectInvocationOf.PropertyValidation
            .Targeting(Item, x => x.ItemProperty)
            .On(Item, Child, Parent, Grandparent);

         Results.ExpectInvocationOf.CollectionPropertyValidation
            .Targeting(Item, x => x.ItemProperty)
            .On(Child, Parent, Grandparent);

         ExpectInvocationOfViewModelValidatorsFromItemToGrandparentBecauseValidationStateChanged();

         Item.Revalidate(x => x.ItemProperty);
         Results.VerifySequenceAndResults();
      }

      [TestMethod]
      public void Revalidate_ExecutesAncestorCollectionViewModelValidators() {
         Results.SetupFailing.CollectionViewModelValidation
            .Targeting(Item)
            .On(Child, Parent, Grandparent);

         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(Item)
            .On(Item, Child, Parent, Grandparent);

         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(Item)
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
            .Targeting(Item)
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