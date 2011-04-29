namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionValidationTests : CollectionValidationFixture {
      [TestMethod]
      public void ItemAddition_PerformsPropertyAndCollectionPropertyValidationForNewItem() {
         var newItem = CreateItem();
         SetupPropertyValidationError(newItem);
         SetupCollectionPropertyValidationError(newItem);

         List.Items.Add(newItem);

         Setup.VerifySetupValidationResults();
      }

      [TestMethod]
      public void ItemAddition_PerformsViewModelAndCollectionViewModelValidationForNewItem() {
         var newItem = CreateItem();
         SetupViewModelValidationError(newItem);
         SetupCollectionViewModelValidationError(newItem);

         List.Items.Add(newItem);

         Setup.VerifySetupValidationResults();
      }

      [TestMethod]
      public void ItemRemoval_PerformsPropertyValidationForOldItemAndCollectionValidationForRemainingItems() {
         var remainingItem = CreateItem("RemainingItem");
         var oldItem = CreateItem("OldItem");

         List.Items.Add(remainingItem);
         List.Items.Add(oldItem);

         SetupPropertyValidationError(oldItem);
         SetupCollectionPropertyValidationError(remainingItem);

         List.Items.Remove(oldItem);

         Setup.VerifySetupValidationResults();
      }

      [TestMethod]
      public void ItemRemoval_PerformsViewModelValidationForOldItemAndCollectionValidationForRemainingItems() {
         var remainingItem = CreateItem("RemainingItem");
         var oldItem = CreateItem("OldItem");

         List.Items.Add(remainingItem);
         List.Items.Add(oldItem);

         SetupViewModelValidationError(oldItem);
         SetupCollectionViewModelValidationError(remainingItem);

         List.Items.Remove(oldItem);

         Setup.VerifySetupValidationResults();
      }

      [TestMethod]
      public void SetItem_PerformPropertyValidationForOldAndNewItemAndCollectionValidationForNewItems() {
         var oldItem = CreateItem("OldItem");
         var newItem = CreateItem("NewItem");
         List.Items.Add(oldItem);

         SetupPropertyValidationError(oldItem);
         SetupPropertyValidationError(newItem);
         SetupCollectionPropertyValidationError(newItem);

         List.Items[0] = newItem;

         Setup.VerifySetupValidationResults();
      }

      [TestMethod]
      public void SetItem_PerformsViewModelValidationForOldAndNewItemAndCollectionValidationForNewItems() {
         var oldItem = CreateItem("OldItem");
         var newItem = CreateItem("NewItem");
         List.Items.Add(oldItem);

         SetupViewModelValidationError(oldItem);
         SetupViewModelValidationError(newItem);
         SetupCollectionViewModelValidationError(newItem);

         List.Items[0] = newItem;

         Setup.VerifySetupValidationResults();
      }

      [TestMethod]
      public void Clear_PerformsPropertyValidationForOldItemsAndCollectionValidationForEmptyCollection() {
         var oldItem = CreateItem();
         List.Items.Add(oldItem);

         SetupPropertyValidationError(oldItem);
         List.Items.Clear();

         Setup.VerifySetupValidationResults();
         Assert.IsTrue(Setup.ActualInvocations.Any(x => x.TargetCollection == List.Items));
      }

      [TestMethod]
      public void Clear_PerformsViewModelValidationForOldItemsAndCollectionValidationForEmptyCollection() {
         var oldItem = CreateItem();
         List.Items.Add(oldItem);

         SetupViewModelValidationError(oldItem);
         List.Items.Clear();

         Assert.IsTrue(Setup.ActualInvocations.Any(x => x.TargetCollection == List.Items));
      }

      [TestMethod]
      public void SetItems_DoesNotValidateAnything() {
         List.Items.Add(CreateItem("Old item"));
         Setup.Reset();

         List.Items.ReplaceItems(new[] { CreateItem("New item") });

         Assert.AreEqual(0, Setup.ActualInvocations.Count);
      }

      private void SetupCollectionPropertyValidationError(ItemVM item) {
         Setup.SetupFailing().CollectionPropertyValidation
            .Targeting(item, x => x.CollectionProperty)
            .On(List);
      }

      private void SetupCollectionViewModelValidationError(ItemVM item) {
         Setup.SetupFailing().CollectionViewModelValidation
            .Targeting(item)
            .On(List);
      }

      private void SetupPropertyValidationError(ItemVM item) {
         Setup.SetupFailing().PropertyValidation
            .Targeting(item, x => x.CollectionProperty)
            .On(item);
      }

      private void SetupViewModelValidationError(ItemVM item) {
         Setup.SetupFailing().ViewModelValidation
            .Targeting(item)
            .On(item);
      }
   }
}
