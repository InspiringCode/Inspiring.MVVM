namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionRevalidationTests : CollectionValidationFixture {
      [TestMethod]
      public void RevalidateItem_PerformsCollectionPropertyValidationForThatItem() {
         var item = CreateItem();
         List.Items.Add(item);

         Setup.SetupCollectionPropertyError(item, ItemDescriptor.ItemProperty);
         item.Revalidate();

         Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsCollectionPropertyValidationForAllItems() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");
         var three = CreateItem("Item 3");

         List.Items.Add(one);
         List.Items.Add(two);
         List.Items.Add(three);

         Setup.SetupCollectionPropertyError(two, ItemDescriptor.ItemProperty);
         one.Revalidate();

         Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullPropertyValidationForCurrentlyInvalidItems() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");
         var three = CreateItem("Item 3");

         Setup.SetPropertyError(one, ItemDescriptor.ItemProperty);
         Setup.SetPropertyError(two, ItemDescriptor.ItemProperty);

         List.Items.Add(one);
         List.Items.Add(two);
         List.Items.Add(three);

         one.Revalidate();

         Setup.ExpectedValid(one);
         Setup.ExpectedValid(two);

         Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullPropertyValidationForItemsWithInvalidCollectionPropertyValidation() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");

         Setup.SetupCollectionPropertyError(two, ItemDescriptor.ItemProperty);
         Setup.SetupPropertyError(two, ItemDescriptor.ItemProperty);

         one.Revalidate();

         Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsCollectionViewModelValidationForThatItem() {
         var item = CreateItem();
         List.Items.Add(item);
         Setup.SetupCollectionViewModelError(item);
         item.Revalidate();
         Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsCollectionViewModelValidationForAllItems() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");
         var three = CreateItem("Item 3");

         List.Items.Add(one);
         List.Items.Add(two);
         List.Items.Add(three);

         Setup.SetupCollectionViewModelError(two);
         one.Revalidate();

         Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullViewModelValidationForCurrentlyInvalidItems() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");
         var three = CreateItem("Item 3");

         Setup.SetViewModelError(one);
         Setup.SetViewModelError(two);

         List.Items.Add(one);
         List.Items.Add(two);
         List.Items.Add(three);

         one.Revalidate();

         Setup.ExpectedValid(one);
         Setup.ExpectedValid(two);

         Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullViewModelValidationForItemsWithInvalidCollectionViewModelValidation() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");

         Setup.SetupCollectionViewModelError(two);
         Setup.SetupViewModelError(two);

         one.Revalidate();

         Setup.VerifyAll();
      }
   }
}