namespace Inspiring.MvvmTest.ViewModels.Core {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CountingSetTest {

      [TestMethod]
      public void RemoveItem_TheSameItemWasAddedTwice_CollectionStillContainsItem() {
         CountingSet<object> coll = new CountingSet<object>();

         object item = new object();

         coll.Add(item);
         coll.Add(item);

         coll.Remove(item);

         Assert.IsTrue(coll.Contains(item));
      }

      [TestMethod]
      public void RemoveItemTwice_TheSameItemWasAddedTwice_CollectionDoesntContainItem() {
         CountingSet<object> coll = new CountingSet<object>();

         object item = new object();

         coll.Add(item);
         coll.Add(item);

         coll.Remove(item);
         coll.Remove(item);

         Assert.IsFalse(coll.Contains(item));
      }
   }
}
