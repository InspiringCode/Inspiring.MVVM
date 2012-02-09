namespace Inspiring.MvvmTest.ViewModels.Core.Properties.CollectionProperty {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PopulatedCollectionAccessorBehaviorTests : CollectionAccessorBehaviorFixture {
      private PopulatedCollectionAccessorBehavior<ItemVM> Behavior { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new PopulatedCollectionAccessorBehavior<ItemVM>();
         Context = CreateContext(Behavior);
      }

      [TestMethod]
      public void GetValue_Initially_CreatesAndReturnsPopulatedInstance() {
         var expectedItems = new[] { CreateItem() };
         Next.PopulatedItemsToReturn = expectedItems;

         var actual = Behavior.GetValue(Context);

         Assert.AreEqual(Next.CollectionToReturn, actual);
         CollectionAssert.AreEqual(expectedItems, actual.ToArray());
      }

      [TestMethod]
      public void GetValue_Initially_CallsInitializeValue() {
         Behavior.GetValue(Context);
         Behavior.GetValue(Context);
         Assert.AreEqual(1, Next.InitializeValueInvocations);
      }

      [TestMethod]
      public void GetValue_SecondTime_ReturnsCachedInstance() {
         var first = Behavior.GetValue(Context);
         var second = Behavior.GetValue(Context);

         Assert.AreEqual(first, second);
         Assert.AreEqual(1, Next.CreateCollectionInvocations);
         Assert.AreEqual(1, Next.GetPopulatedItemsInvocations);
      }

      [TestMethod]
      public void Refresh_CallsSourceVMAccessor() {
         Next.PopulatedItemsToReturn = new[] { CreateItem() };
         Behavior.GetValue(Context);

         var newSourceVMs = new[] { CreateItem(), CreateItem() };
         Next.PopulatedItemsToReturn = newSourceVMs;

         InvokeRefresh();

         var collection = Behavior.GetValue(Context).ToArray();

         CollectionAssert.AreEqual(newSourceVMs, collection);
         Assert.IsTrue(collection.All(x => x.RefreshInvocations == 0));
      }

      [TestMethod]
      public void Refresh_CallsNextBehavior() {
         InvokeRefresh();
         Assert.AreEqual(1, Next.RefreshInvocations);
      }
      
      private void InvokeRefresh() {
         Behavior.Refresh(Context, new RefreshOptions());
      }
   }
}