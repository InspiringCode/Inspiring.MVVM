namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class MultipleOwnerCollectionsValidationTests : MultipleOwnerCollectionsValidationFixture {
      [TestMethod]
      public void ItemIsValid_NoErrorIsAdded() {
         ItemAB.Revalidate(x => x.ItemProperty); ;
         ValidationAssert.IsValid(ItemAB);
      }

      [TestMethod]
      public void ItemIsValid_ValidatesOnlyOwnerCollectionsOfThatItem() {
         Results.EnabledValidators = ValidatorTypes.Property;

         ItemAB.Revalidate(x => x.ItemProperty); ;

         ExpectItemPropertyValidationOf(ItemAB);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionA);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionB);

         Results.VerifyInvocationSequence();
      }


      [TestMethod]
      public void ItemIsInvalidInSingleCollection_AddsErrorToItem() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         ItemAB.Revalidate(x => x.ItemProperty); ;

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void ItemIsInvalidInSingleCollection_ValidatesOnlyOwnerCollectionsOfThatItem() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         ItemAB.Revalidate(x => x.ItemProperty); ;

         ExpectItemPropertyValidationOf(ItemAB);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionA);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionB);

         Results.VerifyInvocationSequence();
      }

      [TestMethod]
      public void GetResults_ItemIsInvalidInMultipleCollections_AddsErrorsOfAllCollectionsToItem() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionBValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfC, CollectionCValidatorKey);

         ItemABC.Revalidate(x => x.ItemProperty); ;

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void GetResults_ItemIsInvalidInMultipleCollections_ValidatesOnlyOwnerCollectionsOfThatItem() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionBValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfC, CollectionCValidatorKey);

         ItemABC.Revalidate(x => x.ItemProperty); ;

         ExpectItemPropertyValidationOf(ItemABC);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionA);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionB);
         ExpectCollectionPropertyValidationOf(OwnerOfC.CollectionC);

         Results.VerifyInvocationSequence();
      }

      [TestMethod]
      public void MultipleItemsAreInvalidInSingleCollection_OneInvalidItemIsRevalidated_RevalidatesInvalidItemsAndAddsErrors() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemA, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         ItemA.Revalidate(x => x.ItemProperty); ;

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void MultipleItemsAreInvalidInSingleCollection_OneInvalidItemIsRevalidated_ValidatesOwnerCollectionsAndRevalidatesInvalidItems() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemA, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         ItemA.Revalidate(x => x.ItemProperty); ;

         ExpectItemPropertyValidationOf(ItemA);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionA);
         ExpectItemPropertyValidationOf(ItemAB);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionB);

         Results.VerifyInvocationSequence();
      }

      [TestMethod]
      public void Revalidate_OtherItemBecomesInvalid_RevalidatesOtherItem() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().PropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(ItemABC);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         ItemA.Revalidate(x => x.ItemProperty); ;
         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void Revalidate_OtherItemBecomesInvalid_ValidatesUnvalidatedOwnerCollectionsOfOtherItems() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         ItemA.Revalidate(x => x.ItemProperty); ;

         ExpectItemPropertyValidationOf(ItemA);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionA);

         ExpectItemPropertyValidationOf(ItemABC);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionB);
         ExpectCollectionPropertyValidationOf(OwnerOfC.CollectionC);

         Results.VerifyInvocationSequence();
      }


      [TestMethod]
      public void Revalidate_OtherItemWasPreviouslyInvalid_RevalidatesOtherItem() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetFailed("Previous error").PropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(ItemABC);

         Results.SetupFailing("New error").PropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(ItemABC);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         ItemA.Revalidate(x => x.ItemProperty); ;

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void Revalidate_OtherItemWasPreviouslyInvalid_ValidatesUnvalidatedCollectionsOfOtherItem() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetFailed("Previous error").CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         ItemA.Revalidate(x => x.ItemProperty);

         ExpectItemPropertyValidationOf(ItemA);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionA);

         ExpectItemPropertyValidationOf(ItemABC);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionB);
         ExpectCollectionPropertyValidationOf(OwnerOfC.CollectionC);

         Results.VerifyInvocationSequence();
      }

      [TestMethod]
      public void AllItemsBecomeInvalid_RevalidatesAllItems() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemA, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionBValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionBValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfC, CollectionCValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemC, x => x.ItemProperty)
            .On(OwnerOfC, CollectionCValidatorKey);

         ItemAB.Revalidate(x => x.ItemProperty); ;

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void AllItemsBecomeInvalid_ValidatesEachCollectionOnlyOnce() {
         Results.EnabledValidators = ValidatorTypes.Property;

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemA, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionBValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionBValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(OwnerOfC, CollectionCValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemC, x => x.ItemProperty)
            .On(OwnerOfC, CollectionCValidatorKey);

         ItemAB.Revalidate(x => x.ItemProperty);

         ExpectItemPropertyValidationOf(ItemAB);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionA);
         ExpectCollectionPropertyValidationOf(OwnerOfAB.CollectionB);

         ExpectItemPropertyValidationOf(ItemA);

         ExpectItemPropertyValidationOf(ItemABC);
         ExpectCollectionPropertyValidationOf(OwnerOfC.CollectionC);
         ExpectItemPropertyValidationOf(ItemC);

         Results.VerifyInvocationSequence();
      }

      [TestMethod]
      public void TwoPropertiesOfAnItemBecomeInvalid_CorrectErrorsAreAddedToBoth() {
         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemA, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemA, x => x.SecondItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.SecondItemProperty)
            .On(OwnerOfAB, CollectionAValidatorKey);

         OwnerOfAB.Revalidate(x => x.CollectionA, ValidationScope.SelfAndAllDescendants);

         Results.VerifySetupValidationResults();
      }

      private void ExpectItemPropertyValidationOf(ItemVM item) {
         Results.ExpectInvocationOf.PropertyValidation
            .Targeting(item, x => x.ItemProperty)
            .On(item);
      }

      private void ExpectViewModelValidationOf(ItemVM item) {
         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(item)
            .On(item);
      }

      private void ExpectCollectionPropertyValidationOf(IVMCollection<ItemVM> collection) {
         IViewModel owner;
         string key;

         if (collection == OwnerOfAB.CollectionA) {
            owner = OwnerOfAB;
            key = CollectionAValidatorKey;
         } else if (collection == OwnerOfAB.CollectionB) {
            owner = OwnerOfAB;
            key = CollectionBValidatorKey;
         } else if (collection == OwnerOfC.CollectionC) {
            owner = OwnerOfC;
            key = CollectionCValidatorKey;
         } else {
            throw new InternalTestFailureException();
         }

         Results.ExpectInvocationOf.CollectionPropertyValidation
            .Targeting(collection, x => x.ItemProperty)
            .On(owner, key);
      }
   }
}