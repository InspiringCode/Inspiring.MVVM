namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   /// <remarks>
   ///                      
   ///   <![CDATA[                                  
   ///                                     ___Item1
   ///                                   /   
   ///                                  /       
   ///                 --> Collection1  -------       
   ///                |                 \      |
   ///               VM1                 \     |    
   ///                |                   \    |
   ///                 --> Collection2 ----\--Item2
   ///                                 \    \
   ///                                  \    \
   ///                                   \____Item3 
   ///                                             \
   ///                                              ----- Collection3 <--- VM2
   ///                                             /
   ///                                        Item4
   ///   ]]>                                       
   /// </remarks>
   [TestClass]
   public class CollectionResultCacheTests : CollectionResultCacheFixture {

      public enum Validator {
         Collection1PropertyValidator,
         Collection2PropertyValidator,
         Collection3PropertyValidator,
         Collection1ViewModelValidator,
         Collection2ViewModelValidator,
         Collection3ViewModelValidator,
      }

      public const string Collection1PropertyValidationErrorMessage = "Error of collection1 property validator";
      public const string Collection2PropertyValidationErrorMessage = "Error of collection2 property validator";
      public const string Collection3PropertyValidationErrorMessage = "Error of collection3 property validator";
      public const string Collection1ViewModelValidationErrorMessage = "Error of collection1 view model validator";
      public const string Collection2ViewModelValidationErrorMessage = "Error of collection2 view model validator";
      public const string Collection3ViewModelValidationErrorMessage = "Error of collection3 view model validator";
      public const string NamePropertyValidatorErrorMessage = "Error of name property validator";

      private const string Item1Name = "Item1";
      private const string Item2Name = "Item2";
      private const string Item3Name = "Item3";
      private const string Item4Name = "Item4";

      private TwoItemListsVM VM1;
      private ItemListVM VM2;

      private ItemVM Item1 { get; set; }
      private ItemVM Item2 { get; set; }
      private ItemVM Item3 { get; set; }
      private ItemVM Item4 { get; set; }

      private CollectionResultCache ResultCache { get; set; }

      public List<ItemVM> InvalidItemsOfItemVMPropertyValidator { get; private set; }
      public List<ItemVM> InvalidItemsOfCollection1PropertyValidator { get; private set; }
      public List<ItemVM> InvalidItemsOfCollection1ViewModelValidator { get; private set; }
      public List<ItemVM> InvalidItemsOfCollection2PropertyValidator { get; private set; }
      public List<ItemVM> InvalidItemsOfCollection2ViewModelValidator { get; private set; }
      public List<ItemVM> InvalidItemsOfCollection3PropertyValidator { get; private set; }
      public List<ItemVM> InvalidItemsOfCollection3ViewModelValidator { get; private set; }

      public ValidatorInvocationLog InvocationLog { get; private set; }

      [TestInitialize]
      public void Setup() {
         InvalidItemsOfItemVMPropertyValidator = new List<ItemVM>();
         InvalidItemsOfCollection1PropertyValidator = new List<ItemVM>();
         InvalidItemsOfCollection1ViewModelValidator = new List<ItemVM>();
         InvalidItemsOfCollection2PropertyValidator = new List<ItemVM>();
         InvalidItemsOfCollection2ViewModelValidator = new List<ItemVM>();
         InvalidItemsOfCollection3PropertyValidator = new List<ItemVM>();
         InvalidItemsOfCollection3ViewModelValidator = new List<ItemVM>();

         Item1 = new ItemVM(this, Item1Name);
         Item2 = new ItemVM(this, Item2Name);
         Item3 = new ItemVM(this, Item3Name);
         Item4 = new ItemVM(this, Item4Name);

         InvocationLog = new ValidatorInvocationLog();

         VM1 = new TwoItemListsVM(this, InvocationLog);
         VM2 = new ItemListVM(this, InvocationLog);

         VM1.GetValue(x => x.Collection1).Add(Item1);
         VM1.GetValue(x => x.Collection1).Add(Item2);
         VM1.GetValue(x => x.Collection1).Add(Item3);

         VM1.GetValue(x => x.Collection2).Add(Item2);
         VM1.GetValue(x => x.Collection2).Add(Item3);

         VM2.GetValue(x => x.Collection3).Add(Item3);
         VM2.GetValue(x => x.Collection3).Add(Item4);

         ResultCache = new CollectionResultCache();
      }

      [TestMethod]
      public void GetResults_ItemIsValid_ReturnsNoError() {
         var result = ResultCache.GetCollectionValidationResults(
            ValidationStep.Value,
            Item3,
            null
         );
         Assert.IsTrue(result.IsValid);
      }

      [TestMethod]
      public void GetResults_ItemIsInvalidInSingleCollection_ReturnsError() {
         ChangeValidatorResult(InvalidItemsOfCollection1PropertyValidator, Item2, false);

         var result = ResultCache.GetCollectionValidationResults(
            ValidationStep.Value,
            Item2,
            ItemVM.ClassDescriptor.Name
         );

         var expectedError = new ValidationError(
            null,
            Item2,
            ItemVM.ClassDescriptor.Name,
            Collection1PropertyValidationErrorMessage);

         AssertErrors(result.Errors, expectedError);
      }

      [TestMethod]
      public void GetResults_ItemIsInvalidInMultipleCollections_ReturnsErrorsForAllCollections() {
         ChangeValidatorResult(InvalidItemsOfCollection1PropertyValidator, Item3, false);
         ChangeValidatorResult(InvalidItemsOfCollection2PropertyValidator, Item3, false);
         ChangeValidatorResult(InvalidItemsOfCollection3PropertyValidator, Item3, false);

         var result = ResultCache.GetCollectionValidationResults(
           ValidationStep.Value,
           Item3,
           ItemVM.ClassDescriptor.Name
         );

         var expectedErrors = new ValidationError[] {
            new ValidationError(null, Item3, ItemVM.ClassDescriptor.Name, Collection1PropertyValidationErrorMessage),
            new ValidationError(null, Item3, ItemVM.ClassDescriptor.Name, Collection2PropertyValidationErrorMessage),
            new ValidationError(null, Item3, ItemVM.ClassDescriptor.Name, Collection3PropertyValidationErrorMessage)
         };

         AssertErrors(result.Errors, expectedErrors);
      }

      [TestMethod]
      public void GetResults_MultipleItemsAreInvalidInSingleCollection_ReturnsOnlyErrorsForSpecifiedItem() {
         Assert.Inconclusive("What should this test test?");
         ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, Item1, false);
         ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, Item2, false);
         Item1.Revalidate();
         Item2.Revalidate();

         var result = ResultCache.GetCollectionValidationResults(
           ValidationStep.Value,
           Item1,
           ItemVM.ClassDescriptor.Name
         );

         var expectedError = new ValidationError(
            null,
            Item1,
            ItemVM.ClassDescriptor.Name,
            NamePropertyValidatorErrorMessage);

         AssertErrors(result.Errors, expectedError);
      }

      [TestMethod]
      public void GetResults_OtherItemBecomesInvalid_ValidatesItsUnvalidatedOwnerCollectionsToo() {
         ChangeValidatorResult(InvalidItemsOfCollection1ViewModelValidator, Item3, false);

         InvocationLog.ExpectCalls(
            Validator.Collection1ViewModelValidator,
            Validator.Collection2ViewModelValidator,
            Validator.Collection3ViewModelValidator
         );

         ResultCache.GetCollectionValidationResults(
            ValidationStep.ViewModel,
            Item1,
            null
         );

         InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResults_OtherItemBecomesInvalid_DoesNotValidateAlreadyValidatedOwnerCollections() {
         ChangeValidatorResult(InvalidItemsOfCollection3PropertyValidator, Item4, false);

         InvocationLog.ExpectCalls(
            Validator.Collection1PropertyValidator,
            Validator.Collection2PropertyValidator,
            Validator.Collection3PropertyValidator
         );

         ResultCache.GetCollectionValidationResults(
            ValidationStep.Value,
            Item3,
            ItemVM.ClassDescriptor.Name
         );

         InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResults_OtherItemWasPreviouslyInvalid_ValidatesItsUnvalidatedOwnerCollectionsToo() {
         ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, Item3, false);
         Item3.Revalidate();

         InvocationLog.ExpectCalls(
            Validator.Collection3PropertyValidator,
            Validator.Collection1PropertyValidator,
            Validator.Collection2PropertyValidator
         );

         ResultCache.GetCollectionValidationResults(
            ValidationStep.Value,
            Item4,
            ItemVM.ClassDescriptor.Name
         );

         InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResults_OtherItemWasPreviouslyInvalid_DoesNotValidateAlreadyValidatedOwnerCollections() {
         ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, Item2, false);
         Item2.Revalidate();

         InvocationLog.ExpectCalls(
            Validator.Collection1ViewModelValidator,
            Validator.Collection2ViewModelValidator,
            Validator.Collection3ViewModelValidator
         );

         ResultCache.GetCollectionValidationResults(
            ValidationStep.ViewModel,
            Item3,
            null
         );

         InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResults_AllItemsBecomeInvalid_InvokesCollectionsValidatorsOnlyOnce() {
         ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, Item1, false);
         Item1.Revalidate();

         ChangeValidatorResult(InvalidItemsOfItemVMPropertyValidator, Item2, false);
         Item2.Revalidate();

         ChangeValidatorResult(InvalidItemsOfCollection1PropertyValidator, Item3, false);
         ChangeValidatorResult(InvalidItemsOfCollection3PropertyValidator, Item4, false);

         InvocationLog.ExpectCalls(
            Validator.Collection1PropertyValidator,
            Validator.Collection2PropertyValidator,
            Validator.Collection3PropertyValidator
         );

         ResultCache.GetCollectionValidationResults(
            ValidationStep.Value,
            Item3,
            ItemVM.ClassDescriptor.Name
         );

         InvocationLog.VerifyCalls();
      }

      [TestMethod]
      public void GetResultsForMultipleItemsOfSameCollection_SubsequentCallsUseCache() {
         InvocationLog.ExpectCalls(
            Validator.Collection1PropertyValidator,
            Validator.Collection2PropertyValidator,
            Validator.Collection3PropertyValidator
         );

         ResultCache.GetCollectionValidationResults(
           ValidationStep.Value,
           Item3,
           ItemVM.ClassDescriptor.Name
         );

         ResultCache.GetCollectionValidationResults(
           ValidationStep.Value,
           Item2,
           ItemVM.ClassDescriptor.Name
         );

         InvocationLog.VerifyCalls();
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