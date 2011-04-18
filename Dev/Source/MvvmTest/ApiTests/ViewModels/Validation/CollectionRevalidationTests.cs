namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionRevalidationTests : CollectionValidationFixture {
      [TestMethod]
      public void RevalidateItem_PerformsCollectionPropertyValidationForThatItem() {
         var item = CreateItem();
         List.Items.Add(item);

         Setup.CollectionPropertyError(item, ItemDescriptor.ItemProperty);
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

         Setup.CollectionPropertyError(two, ItemDescriptor.ItemProperty);
         one.Revalidate();

         Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullPropertyValidationForCurrentlyInvalidItems() {
         var one = CreateItem("Item 1");
         var two = CreateItem("Item 2");
         var three = CreateItem("Item 3");

         Setup.PropertyError(one, ItemDescriptor.ItemProperty);
         Setup.PropertyError(two, ItemDescriptor.ItemProperty);

         one.Revalidate();
         two.Revalidate();

         Setup.Reset();

         List.Items.Add(one);
         List.Items.Add(two);
         List.Items.Add(three);

         one.Revalidate();

         ValidationAssert.IsValid(one);
         ValidationAssert.IsValid(two);         
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullPropertyValidationForItemsWithInvalidCollectionPropertyValidation() {

      }

      [TestMethod]
      public void RevalidateItem_PerformsCollectionViewModelValidationForThatItem() {
         var item = CreateItem();
         List.Items.Add(item);
         Setup.CollectionViewModelError(item);
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

         Setup.CollectionViewModelError(two);
         one.Revalidate();

         Setup.VerifyAll();
      }

      [TestMethod]
      public void RevalidateItem_PerformsFullViewModelValidationForCurrentlyInvalidItems() {

      }

      [TestMethod]
      public void RevalidateItem_PerformsFullViewModelValidationForItemsWithInvalidCollectionViewModelValidation() {

      }
   }
}