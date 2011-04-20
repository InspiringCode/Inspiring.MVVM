namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionResultCacheTests : CollectionResultCacheFixture {
      private CollectionResultCache ResultCache { get; set; }

      private ValidationResult ExpectedResult {
         get {
            return new ValidationResult(Results.SetupResults.SelectMany(x => x.Errors));
         }
      }

      [TestInitialize]
      public void Setup() {
         ResultCache = new CollectionResultCache();
      }

      [TestMethod]
      public void GetResults_ItemIsValid_ReturnsNoError() {
         var result = ResultCache.GetCollectionValidationResults(
            ValidationStep.Value,
            ItemABC,
            property: null
         );

         Results.VerifyValidationResults();
         ValidationAssert.IsValid(result);
      }

      [TestMethod]
      public void GetResults_ItemIsInvalidInSingleCollection_ReturnsError() {
         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(Owner1, CollectionAValidatorKey);

         var result = ResultCache.GetCollectionValidationResults(
            ValidationStep.Value,
            ItemAB,
            ItemVM.ClassDescriptor.ItemProperty
         );

         ValidationAssert.AreEqual(ExpectedResult, result);

         //ChangeValidatorResult(InvalidItemsOfCollection1PropertyValidator, ItemAB, false);

         //var result = ResultCache.GetCollectionValidationResults(
         //   ValidationStep.Value,
         //   ItemAB,
         //   ItemVM.ClassDescriptor.ItemProperty
         //);

         //var expectedError = new ValidationError(
         //   null,
         //   ItemAB,
         //   ItemVM.ClassDescriptor.ItemProperty,
         //   Collection1PropertyValidationErrorMessage);

         //AssertErrors(result.Errors, expectedError);
      }

      [TestMethod]
      public void GetResults_ItemIsInvalidInMultipleCollections_ReturnsErrorsForAllCollections() {
         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(Owner1, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(Owner1, CollectionBValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemABC, x => x.ItemProperty)
            .On(Owner2, CollectionCValidatorKey);

         //ChangeValidatorResult(InvalidItemsOfCollection1PropertyValidator, ItemABC, false);
         //ChangeValidatorResult(InvalidItemsOfCollection2PropertyValidator, ItemABC, false);
         //ChangeValidatorResult(InvalidItemsOfCollection3PropertyValidator, ItemABC, false);

         var result = ResultCache.GetCollectionValidationResults(
           ValidationStep.Value,
           ItemABC,
           ItemVM.ClassDescriptor.ItemProperty
         );

         //var expectedErrors = new ValidationError[] {
         //   new ValidationError(null, ItemABC, ItemVM.ClassDescriptor.ItemProperty, Collection1PropertyValidationErrorMessage),
         //   new ValidationError(null, ItemABC, ItemVM.ClassDescriptor.ItemProperty, Collection2PropertyValidationErrorMessage),
         //   new ValidationError(null, ItemABC, ItemVM.ClassDescriptor.ItemProperty, Collection3PropertyValidationErrorMessage)
         //};

         //AssertErrors(result.Errors, expectedErrors);

         ValidationAssert.AreEqual(ExpectedResult, result);
      }

      [TestMethod]
      public void GetResults_MultipleItemsAreInvalidInSingleCollection_ReturnsOnlyErrorsForSpecifiedItem() {
         //Assert.Inconclusive("What should this test test?");

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemA, x => x.ItemProperty)
            .On(Owner1, CollectionAValidatorKey);

         Results.SetupFailing().CollectionPropertyValidation
            .Targeting(ItemAB, x => x.ItemProperty)
            .On(Owner1, CollectionAValidatorKey);

         var result = ResultCache.GetCollectionValidationResults(
           ValidationStep.Value,
           ItemA,
           ItemVM.ClassDescriptor.ItemProperty
         );

         var expectedResult = new ValidationResult(Results
            .SetupResults
            .SelectMany(x => x.Errors)
            .Where(x => x.Target == ItemAB));

         ValidationAssert.AreEqual(expectedResult, result);


         //ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, ItemA, false);
         //ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, ItemAB, false);
         //ItemA.Revalidate();
         //ItemAB.Revalidate();

         //var result = ResultCache.GetCollectionValidationResults(
         //  ValidationStep.Value,
         //  ItemA,
         //  ItemVM.ClassDescriptor.ItemProperty
         //);

         //var expectedError = new ValidationError(
         //   null,
         //   ItemA,
         //   ItemVM.ClassDescriptor.ItemProperty,
         //   NamePropertyValidatorErrorMessage);

         //AssertErrors(result.Errors, expectedError);
      }

      [TestMethod]
      public void GetResults_OtherItemBecomesInvalid_ValidatesItsUnvalidatedOwnerCollectionsToo() {
         Results.SetupFailing().CollectionViewModelValidation
            .Targeting(ItemABC)
            .On(Owner1, CollectionAValidatorKey);

         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(ItemA)
            .On(Owner1, CollectionAValidatorKey);

         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(ItemAB)
            .On(Owner1, CollectionAValidatorKey);

         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(ItemABC)
            .On(Owner1, CollectionAValidatorKey);


         Results.ExpectInvocationOf.ViewModelValidation
            .Targeting(ItemABC)
            .On(ItemABC);


         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(ItemAB)
            .On(Owner1, CollectionBValidatorKey);

         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(ItemABC)
            .On(Owner1, CollectionBValidatorKey);

         
         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(ItemABC)
            .On(Owner2, CollectionCValidatorKey);

         Results.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(ItemC)
            .On(Owner2, CollectionCValidatorKey);


         ResultCache.GetCollectionValidationResults(
            ValidationStep.ViewModel,
            ItemA,
            property: null
         );

         Results.VerifyInvocationSequence();

         //ChangeValidatorResult(InvalidItemsOfCollection1ViewModelValidator, ItemABC, false);

         //InvocationLog.ExpectCalls(
         //   Validator.Collection1ViewModelValidator,
         //   Validator.Collection2ViewModelValidator,
         //   Validator.Collection3ViewModelValidator
         //);

         //ResultCache.GetCollectionValidationResults(
         //   ValidationStep.ViewModel,
         //   ItemA,
         //   null
         //);

         //InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResults_OtherItemBecomesInvalid_DoesNotValidateAlreadyValidatedOwnerCollections() {
         //ChangeValidatorResult(InvalidItemsOfCollection3PropertyValidator, ItemC, false);

         //InvocationLog.ExpectCalls(
         //   Validator.Collection1PropertyValidator,
         //   Validator.Collection2PropertyValidator,
         //   Validator.Collection3PropertyValidator
         //);

         //ResultCache.GetCollectionValidationResults(
         //   ValidationStep.Value,
         //   ItemABC,
         //   ItemVM.ClassDescriptor.ItemProperty
         //);

         //InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResults_OtherItemWasPreviouslyInvalid_ValidatesItsUnvalidatedOwnerCollectionsToo() {
         //ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, ItemABC, false);
         //ItemABC.Revalidate();

         //InvocationLog.ExpectCalls(
         //   Validator.Collection3PropertyValidator,
         //   Validator.Collection1PropertyValidator,
         //   Validator.Collection2PropertyValidator
         //);

         //ResultCache.GetCollectionValidationResults(
         //   ValidationStep.Value,
         //   ItemC,
         //   ItemVM.ClassDescriptor.ItemProperty
         //);

         //InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResults_OtherItemWasPreviouslyInvalid_DoesNotValidateAlreadyValidatedOwnerCollections() {
         //ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, ItemAB, false);
         //ItemAB.Revalidate();

         //InvocationLog.ExpectCalls(
         //   Validator.Collection1ViewModelValidator,
         //   Validator.Collection2ViewModelValidator,
         //   Validator.Collection3ViewModelValidator
         //);

         //ResultCache.GetCollectionValidationResults(
         //   ValidationStep.ViewModel,
         //   ItemABC,
         //   null
         //);

         //InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResults_AllItemsBecomeInvalid_InvokesCollectionsValidatorsOnlyOnce() {
         //ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, ItemA, false);
         //ItemA.Revalidate();

         //ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, ItemAB, false);
         //ItemAB.Revalidate();

         //ChangeValidatorResult(InvalidItemsOfCollection1PropertyValidator, ItemABC, false);
         //ChangeValidatorResult(InvalidItemsOfCollection3PropertyValidator, ItemC, false);

         //InvocationLog.ExpectCalls(
         //   Validator.Collection1PropertyValidator,
         //   Validator.Collection2PropertyValidator,
         //   Validator.Collection3PropertyValidator
         //);

         //ResultCache.GetCollectionValidationResults(
         //   ValidationStep.Value,
         //   ItemABC,
         //   ItemVM.ClassDescriptor.ItemProperty
         //);

         //InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResultsForMultipleItemsOfSameCollection_SubsequentCallsUseCache() {
         //InvocationLog.ExpectCalls(
         //   Validator.Collection1PropertyValidator,
         //   Validator.Collection2PropertyValidator,
         //   Validator.Collection3PropertyValidator
         //);

         //ResultCache.GetCollectionValidationResults(
         //  ValidationStep.Value,
         //  ItemABC,
         //  ItemVM.ClassDescriptor.ItemProperty
         //);

         //ResultCache.GetCollectionValidationResults(
         //  ValidationStep.Value,
         //  ItemAB,
         //  ItemVM.ClassDescriptor.ItemProperty
         //);

         //InvocationLog.VerifyCalls();
      }

      private static void AssertErrors(
         IEnumerable<ValidationError> actualErrors,
         params ValidationError[] expectedErrors
      ) {
         CollectionAssert.AreEqual(expectedErrors, actualErrors.ToArray(), new ValidationErrorComparer());
      }

      private void ChangeValidatorResult(IList<ItemVM> invalidItems, ItemVM item, bool isValid) {
         invalidItems.Remove(item);
         if (!isValid) {
            invalidItems.Add(item);
         }
      }

      private class ValidationErrorComparer : IComparer {

         public int Compare(object x, object y) {
            var validationErrorX = (ValidationError)x;
            var validationErrorY = (ValidationError)y;

            if (validationErrorX.Target == validationErrorY.Target &&
                validationErrorX.TargetProperty == validationErrorX.TargetProperty &&
                validationErrorX.Message == validationErrorY.Message) {
               return 0;
            } else {
               return -1;
            }
         }
      }
   }
}