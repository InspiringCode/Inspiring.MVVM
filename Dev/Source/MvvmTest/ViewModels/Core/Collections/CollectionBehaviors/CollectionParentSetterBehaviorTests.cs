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
         Behavior = new ParentSetterCollectionBehavior<IViewModel>();
      }

      [TestMethod]
      public void ItemInserted_SetsParent() {
         IViewModel insertedItem = CreateItem();

         Behavior_ItemInserted(insertedItem);
         Assert.AreSame(CollectionOwner, insertedItem.Kernel.Parent);
      }

      [TestMethod]
      public void ItemRemoved_SetsParentToNull() {
         IViewModel deletedItem = CreateItem();
         deletedItem.Kernel.OwnerCollection = Collection;
         Behavior_ItemRemoved(deletedItem);
         Assert.IsNull(deletedItem.Kernel.Parent);
      }

      [TestMethod]
      public void ItemSet_SetsParentAndClearsParentOfPrevious() {
         var item = CreateItem();
         var previousItem = CreateItem();

         previousItem.Kernel.OwnerCollection = Collection;

         Behavior_ItemSet(previousItem, item);

         Assert.IsNull(previousItem.Kernel.Parent);
         Assert.AreSame(CollectionOwner, item.Kernel.Parent);
      }

      [TestMethod]
      public void ItemsCleared_ClearsParentOfAllItems() {
         var item = CreateItem();
         item.Kernel.OwnerCollection = Collection;

         var previousItems = new IViewModel[] { item };

         Behavior_ItemsCleared(previousItems);
         Assert.IsTrue(previousItems.All(x => x.Kernel.Parent == null));
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
