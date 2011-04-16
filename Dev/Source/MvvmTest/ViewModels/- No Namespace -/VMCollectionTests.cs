namespace Inspiring.MvvmTest.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

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

         var itemDescriptor = DescriptorStub
            .WithBehaviors(itemTypeDescriptorBehavior)
            .Build();

         var ownerProperty = PropertyStub
            .WithBehaviors(new ItemDescriptorProviderBehavior(itemDescriptor))
            .Build();

         var ownerVM = ViewModelStub
            .WithProperties(ownerProperty)
            .Build();

         var collection = new VMCollection<IViewModel>(ViewModelStub.Build(), ownerProperty);

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