using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels.__No_Namespace__ {
   [TestClass]
   public class VMCollectionTests : TestBase {
      [TestMethod]
      public void Move_FromIndexOneToZero_Success() {
         var item0 = CreateItem();
         var item1 = CreateItem();

         var collection = CreateCollection();
         collection.Add(item0);
         collection.Add(item1);

         collection.Move(fromIndex: 1, toIndex: 0);

         CollectionAssert.AreEqual(new IViewModel[] { item1, item0 }, collection);
      }

      [TestMethod]
      public void Move_FromIndexZeroToOne_Success() {
         var item0 = CreateItem();
         var item1 = CreateItem();

         var collection = CreateCollection();
         collection.Add(item0);
         collection.Add(item1);

         collection.Move(fromIndex: 0, toIndex: 1);

         CollectionAssert.AreEqual(new IViewModel[] { item1, item0 }, collection);
      }

      [TestMethod]
      public void GetItemProperties_ReturnsPropertyDescriptorCollection() {
         var itemTypeDescriptorBehavior = new TypeDescriptorProviderBehavior();

         var itemDescriptor = new VMDescriptor();
         itemDescriptor.Behaviors.Successor = itemTypeDescriptorBehavior;
         itemDescriptor.Behaviors.Initialize(itemDescriptor);

         var collectionBehaviors = new BehaviorChain();
         collectionBehaviors.Successor = new ItemDescriptorProviderBehavior(itemDescriptor);

         var collection = CreateCollection();

         Assert.AreSame(itemTypeDescriptorBehavior.PropertyDescriptors, collection.GetItemProperties(null));
      }

      private VMCollection<IViewModel> CreateCollection() {
         return new VMCollection<IViewModel>(ViewModelStub.Build(), PropertyStub.Build());
      }

      private static IViewModel CreateItem() {
         return Mock<IViewModel>();
      }
   }
}