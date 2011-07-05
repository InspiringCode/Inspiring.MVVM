namespace Inspiring.MvvmTest.ViewModels.Core.Properties.CollectionProperty {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ItemInitializerBehaviorTests : CollectionChangeHandlerBehaviorTestBase<IViewModel> {
      private DescriptorStub ItemDescriptor { get; set; }

      [TestInitialize]
      public void Setup() {
         ItemDescriptor = DescriptorStub.Build();
         Behavior = new ItemInitializerBehavior<IViewModel>();

         SetupFixture(Behavior, new ItemDescriptorProviderBehavior(ItemDescriptor));
      }

      [TestMethod]
      public void ItemInserted_AddsParent() {
         var insertedItem = CreateItem("Inserted item");
         Collection = CreateCollection(insertedItem);

         HandleItemInserted(insertedItem);
         CollectionAssert.Contains(insertedItem.Kernel.Parents.ToArray(), CollectionOwner);
      }

      [TestMethod]
      public void ItemRemoved_RemovesParent() {
         IViewModel deletedItem = CreateItem("Deleted item");
         Collection = CreateCollection();
         deletedItem.Kernel.OwnerCollections.Add(Collection);


         HandleItemRemoved(deletedItem);
         CollectionAssert.DoesNotContain(deletedItem.Kernel.Parents.ToArray(), CollectionOwner);
      }

      [TestMethod]
      public void ItemSet_AddsParentToNewAndRemovesParentFromOldItem() {
         var newItem = CreateItem("New item");
         var oldItem = CreateItem("Old item");
         Collection = CreateCollection(newItem);
         oldItem.Kernel.OwnerCollections.Add(Collection);

         HandleItemSet(oldItem, newItem);

         CollectionAssert.DoesNotContain(oldItem.Kernel.Parents.ToArray(), CollectionOwner);
         CollectionAssert.Contains(newItem.Kernel.Parents.ToArray(), CollectionOwner);
      }

      [TestMethod]
      public void CollectionCleared_RemovesParentOfAllItems() {
         var oldItem = CreateItem("Old item");
         Collection = CreateCollection();
         oldItem.Kernel.OwnerCollections.Add(Collection);
         var oldItems = new[] { oldItem };

         HandleCollectionCleared(oldItems);
         CollectionAssert.DoesNotContain(oldItem.Kernel.Parents.ToArray(), CollectionOwner);
      }

      [TestMethod]
      public void CollectionPopulated_RemovesParentFromOldItems() {
         var oldItem = CreateItem("Old item");
         Collection = CreateCollection();
         oldItem.Kernel.OwnerCollections.Add(Collection);
         var oldItems = new[] { oldItem };

         HandleCollectionPopulated(oldItems);
         CollectionAssert.DoesNotContain(oldItem.Kernel.Parents.ToArray(), CollectionOwner);
      }

      [TestMethod]
      public void CollectionPopulated_AddsParentToNewItems() {
         var newItem = CreateItem("New item");
         Collection = CreateCollection(newItem);

         HandleCollectionPopulated(new IViewModel[0]);
         CollectionAssert.Contains(newItem.Kernel.Parents.ToArray(), CollectionOwner);
      }
   }
}
