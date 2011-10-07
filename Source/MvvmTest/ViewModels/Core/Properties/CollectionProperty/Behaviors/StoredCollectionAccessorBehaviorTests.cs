namespace Inspiring.MvvmTest.ViewModels.Core.Properties.CollectionProperty {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class StoredCollectionAccessorBehaviorTests : CollectionAccessorBehaviorFixture {
      private StoredCollectionAccessorBehavior<ItemVM> Behavior { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new StoredCollectionAccessorBehavior<ItemVM>();
         Context = CreateContext(Behavior);
      }

      [TestMethod]
      public void GetValue_Initially_CreatesAndReturnsNewInstance() {
         var expected = CreateCollection();
         Next.CollectionToReturn = expected;

         var actual = Behavior.GetValue(Context);

         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void GetValue_Initially_CallsInitializeValue() {
         Behavior.GetValue(Context);
         Behavior.GetValue(Context);
         Assert.AreEqual(1, Next.InitializeValueInvocations);
      }

      [TestMethod]
      public void GetValue_SecondTime_ReturnsCachedInstance() {
         var expected = CreateCollection();
         Next.CollectionToReturn = expected;
         Behavior.GetValue(Context);
         Next.CollectionToReturn = CreateCollection();

         var second = Behavior.GetValue(Context);

         Assert.AreEqual(expected, second);
         Assert.AreEqual(1, Next.CreateCollectionInvocations);
      }

      [TestMethod]
      public void Refresh_RefreshesAllItems() {
         var item = new ItemVM();
         var collection = Behavior.GetValue(Context);
         collection.Add(item);

         Assert.AreEqual(0, item.RefreshInvocations);
         Behavior.Refresh(Context, false);
         Assert.AreEqual(1, item.RefreshInvocations);
      }

      [TestMethod]
      public void Refresh_CallsNextBehavior() {
         Behavior.Refresh(Context, false);
         Assert.AreEqual(1, Next.RefreshInvocations);
      }
   }
}