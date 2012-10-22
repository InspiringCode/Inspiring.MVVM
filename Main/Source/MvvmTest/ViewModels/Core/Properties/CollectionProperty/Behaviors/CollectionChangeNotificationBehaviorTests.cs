namespace Inspiring.MvvmTest.ViewModels.Core.Properties.CollectionProperty {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionChangeNotificationBehaviorTests : CollectionChangeHandlerBehaviorTestBase<IViewModel> {
      [TestInitialize]
      public void Setup() {
         SetupFixture(new ChangeNotifierCollectionBehavior<IViewModel>());
      }

      [TestMethod]
      public void ItemInserted_CallsNotifyChange() {
         var insertedItem = CreateItem("Inserted item");
         Collection = CreateCollection(insertedItem);

         HandleItemInserted(insertedItem);

         AssertChangeArgs(
            ChangeArgs.ItemsAdded(Collection, newItems: new[] { insertedItem })
         );
      }

      [TestMethod]
      public void ItemRemoved_CallsNotifyChange() {
         var removedItem = CreateItem("Removed item");
         Collection = CreateCollection();

         HandleItemRemoved(removedItem);

         AssertChangeArgs(ChangeArgs.ItemsRemoved(Collection, new[] { removedItem }));
      }

      [TestMethod]
      public void ItemSet_CallsNotifyChangeForOldAndNewItem() {
         var oldItem = CreateItem("Old item");
         var newItem = CreateItem("New item");
         Collection = CreateCollection(newItem);

         HandleItemSet(previousItem: oldItem, item: newItem);

         AssertChangeArgs(
            ChangeArgs.ItemsRemoved(Collection, oldItems: new[] { oldItem }),
            ChangeArgs.ItemsAdded(Collection, newItems: new[] { newItem })
         );
      }

      [TestMethod]
      public void ItemsCleared_CallsNotifyChange() {
         var oldItems = new[] { 
            CreateItem("Old item 1"),
            CreateItem("Old item 2") 
         };
         Collection = CreateCollection();

         HandleCollectionCleared(previousItems: oldItems);

         AssertChangeArgs(
            ChangeArgs.ItemsRemoved(Collection, oldItems: oldItems)
         );
      }

      [TestMethod]
      public void ReplaceItems_CallsNotifyChangeForOldAndNewItems() {
         var oldItems = new[] { 
            CreateItem("Old item 1"),
            CreateItem("Old item 2") 
         };

         var newItems = new[] { 
            CreateItem("New item") 
         };

         Collection = CreateCollection(newItems);

         HandleCollectionPopulated(oldItems);

         AssertChangeArgs(
            ChangeArgs.CollectionPopulated(Collection, oldItems)
         );
      }

      private void AssertChangeArgs(params ChangeArgs[] expected) {
         DomainAssert.AreEqual(expected, Context.NotifyChangeInvocations);
      }
   }
}
