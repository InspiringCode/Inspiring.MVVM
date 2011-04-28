namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   // TODO: Improve tests...

   [TestClass]
   public class CollectionRevalidationTests : CollectionValidationFixture {
      [TestMethod]
      public void RevalidateItemProperty_WhenPropertiesOfAllItemsAreValid_PerformsPropertyAndCollectionValidations() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");

         List.Items.Add(one);
         List.Items.Add(two);

         Setup.Reset();

         // This should not influence the invocation sequence
         Setup.SetFailed().ViewModelValidation
            .Targeting(two)
            .On(two);

         Setup.ExpectInvocationOf.PropertyValidation
            .Targeting(one, x => x.CollectionProperty)
            .On(one);

         Setup.ExpectInvocationOf.CollectionPropertyValidation
            .Targeting(List.Items, x => x.CollectionProperty)
            .On(List);

         one.Revalidate(x => x.CollectionProperty);

         Setup.VerifyInvocationSequence();
      }

      [TestMethod]
      public void RevalidateItemViewModel_WhenViewModelValidationsOfAllItemsAreValid_PerformsViewModelAndCollectionValidations() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");

         List.Items.Add(one);
         List.Items.Add(two);

         Setup.Reset();

         // This should not influence the invocation sequence
         Setup.SetFailed().PropertyValidation
            .Targeting(two, x => x.CollectionProperty)
            .On(two);

         Setup.ExpectInvocationOf.ViewModelValidation
            .Targeting(one)
            .On(one);

         Setup.ExpectInvocationOf.CollectionViewModelValidation
            .Targeting(List.Items)
            .On(List);

         one.RevalidateViewModelValidations();

         Setup.VerifyInvocationSequence();
      }

      [TestMethod]
      public void RevalidateItemProperty_WhenItemIsInvalid_AddsPropertyAndCollectionValidationErrorsToItem() {
         var one = CreateItem("Item 1");
         List.Items.Add(one);

         Setup.Reset();

         Setup.SetupFailing().PropertyValidation
            .Targeting(one, x => x.CollectionProperty)
            .On(one);

         Setup.SetupFailing().CollectionPropertyValidation
            .Targeting(one, x => x.CollectionProperty)
            .On(List);

         one.Revalidate(x => x.CollectionProperty);

         Setup.VerifyValidationResults();
      }

      [TestMethod]
      public void RevalidateItemViewModel_WhenItemIsInvalid_AddsPropertyAndCollectionValidationErrorsToItem() {
         var one = CreateItem("Item 1");
         List.Items.Add(one);

         Setup.Reset();

         Setup.SetupFailing().ViewModelValidation
            .Targeting(one)
            .On(one);

         Setup.SetupFailing().CollectionViewModelValidation
            .Targeting(one)
            .On(List);

         one.RevalidateViewModelValidations();

         Setup.VerifyValidationResults();
      }

      [TestMethod]
      public void RevalidateProperty_WhenCollectionPropertyValidationOfOtherItemIsInvalid_RevalidatesPropertyOfOtherItem() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");

         List.Items.Add(one);
         List.Items.Add(two);

         Setup.Reset();

         Setup.SetFailed().CollectionPropertyValidation
            .Targeting(two, x => x.CollectionProperty)
            .On(List);

         Setup.SetupSucceeding.CollectionPropertyValidation
            .Targeting(two, x => x.CollectionProperty)
            .On(List);

         one.Revalidate(x => x.CollectionProperty);

         Setup.VerifyValidationResults();
      }

      [TestMethod]
      public void RevalidateProperty_WhenViewModelValidationOfOtherItemIsInvalid_DoesNotRevalidateOtherItem() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");

         List.Items.Add(one);
         List.Items.Add(two);

         Setup.Reset();

         Setup.SetFailed().ViewModelValidation
            .Targeting(two)
            .On(two);

         one.Revalidate(x => x.CollectionProperty);

         Assert.IsFalse(Setup.ActualInvocations.Any(x => x.TargetVM == two));
      }

      [TestMethod]
      public void RevalidateItemViewModel_WhenCollectionViewModelValidationOfOtherItemIsInvalid_RevalidatesViewModelOfOtherItem() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");

         List.Items.Add(one);
         List.Items.Add(two);

         Setup.Reset();

         Setup.SetFailed().CollectionViewModelValidation
            .Targeting(two)
            .On(List);

         Setup.SetupSucceeding.CollectionViewModelValidation
            .Targeting(two)
            .On(List);

         one.RevalidateViewModelValidations();

         Setup.VerifyValidationResults();
      }

      [TestMethod]
      public void RevalidateItemViewModel_WhenPropertyValidationOfOtherItemIsInvalid_DoesNotRevalidateOtherItem() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");

         List.Items.Add(one);
         List.Items.Add(two);

         Setup.Reset();

         Setup.SetFailed().PropertyValidation
            .Targeting(two, x => x.CollectionProperty)
            .On(two);

         one.RevalidateViewModelValidations();

         Assert.IsFalse(Setup.ActualInvocations.Any(x => x.TargetVM == two));
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullPropertyValidationForCurrentlyInvalidItems() {
         //var one = CreateItem("Item 1");
         //var two = CreateItem("Item 2");
         //var three = CreateItem("Item 3");

         //Setup.SetPropertyError(one, ItemDescriptor.ItemProperty);
         //Setup.SetPropertyError(two, ItemDescriptor.ItemProperty);

         //List.Items.Add(one);
         //List.Items.Add(two);
         //List.Items.Add(three);

         //Setup.Reset();

         //one.Revalidate();

         //Setup.ExpectedValid(one);
         //Setup.ExpectedValid(two);

         //Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullPropertyValidationForItemsWithInvalidCollectionPropertyValidation() {
         //var one = CreateItem("Item 1");
         //var two = CreateItem("Item 2");

         //Setup.SetupCollectionPropertyError(two, ItemDescriptor.ItemProperty);
         //Setup.SetupPropertyError(two, ItemDescriptor.ItemProperty);

         //one.Revalidate();

         //Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsCollectionViewModelValidationForAllItems() {
         //var one = CreateItem("Item 1");
         //var two = CreateItem("Item 2");
         //var three = CreateItem("Item 3");

         //List.Items.Add(one);
         //List.Items.Add(two);
         //List.Items.Add(three);

         //Setup.SetupCollectionViewModelError(two);
         //one.Revalidate();

         //Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullViewModelValidationForCurrentlyInvalidItems() {
         //var one = CreateItem("Item 1");
         //var two = CreateItem("Item 2");
         //var three = CreateItem("Item 3");

         //Setup.SetViewModelError(one);
         //Setup.SetViewModelError(two);

         //List.Items.Add(one);
         //List.Items.Add(two);
         //List.Items.Add(three);

         //one.Revalidate();

         //Setup.ExpectedValid(one);
         //Setup.ExpectedValid(two);

         //Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullViewModelValidationForItemsWithInvalidCollectionViewModelValidation() {
         //var one = CreateItem("Item 1");
         //var two = CreateItem("Item 2");

         //Setup.SetupCollectionViewModelError(two);
         //Setup.SetupViewModelError(two);

         //one.Revalidate();

         //Setup.VerifyAll();
      }
   }
}