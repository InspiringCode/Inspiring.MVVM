namespace Inspiring.MvvmTest.ViewModels.Core.Collections {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionParentSetterBehaviorTests : CollectionModificationBehaviorTestBase<IViewModel> {
      [TestInitialize]
      public override void Setup() {
         base.Setup();
         Behavior = new ItemInitializerBehavior<IViewModel>();
      }

      [TestMethod]
      public void ItemInserted_SetsParent() {
         IViewModel insertedItem = CreateItem();

         Behavior_ItemInserted(insertedItem);

         Assert.IsTrue(insertedItem.Kernel.Parents.Contains(CollectionOwner));
      }

      [TestMethod]
      public void ItemRemoved_SetsParentToNull() {
         IViewModel deletedItem = CreateItem();
         deletedItem.Kernel.OwnerCollections.Add(Collection);
         Behavior_ItemRemoved(deletedItem);

         Assert.AreEqual(0, deletedItem.Kernel.Parents.Count());
      }

      [TestMethod]
      public void ItemSet_SetsParentAndClearsParentOfPrevious() {
         var item = CreateItem();
         var previousItem = CreateItem();

         previousItem.Kernel.OwnerCollections.Add(Collection);

         Behavior_ItemSet(previousItem, item);

         Assert.AreEqual(0, previousItem.Kernel.Parents.Count());
         Assert.IsTrue(item.Kernel.Parents.Contains(CollectionOwner));
      }

      [TestMethod]
      public void ItemsCleared_ClearsParentOfAllItems() {
         var item = CreateItem();
         item.Kernel.OwnerCollections.Add(Collection);

         var previousItems = new IViewModel[] { item };

         Behavior_ItemsCleared(previousItems);
         Assert.IsTrue(previousItems.All(x => x.Kernel.Parents.Count() == 0));
      }

      [TestMethod]
      public void ReplaceItems_ClearsParentOfAllItems() {
         Assert.Inconclusive();
      }

      protected override IViewModel CreateAnonymousItem() {
         return CreateItem();
      }
   }
}
