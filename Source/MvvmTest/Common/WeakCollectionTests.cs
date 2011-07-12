namespace Inspiring.MvvmTest.Common {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class WeakCollectionTests {
      private ICollection<Item> Collection { get; set; }

      [TestInitialize]
      public void Setup() {
         Collection = new WeakCollection<Item>();
      }

      [TestMethod]
      public void Add_AddsItem() {
         var item = new Item(0);

         Collection.Add(item);

         CollectionAssert.AreEquivalent(
            new[] { item },
            Collection.ToArray()
         );

         GC.KeepAlive(item);
      }

      [TestMethod]
      public void Remove_RemovesItem() {
         var item = new Item(0);

         Collection.Add(item);
         Collection.Remove(item);

         CollectionAssert.AreEquivalent(
            new Item[0],
            Collection.ToArray()
         );

         GC.KeepAlive(item);
      }

      [TestMethod]
      public void Remove_OfNotContainedItem_ReturnsFalse() {
         var notContainedItem = new Item(-1);
         Collection.Add(new Item(0));

         Assert.IsFalse(Collection.Remove(notContainedItem));
      }

      [TestMethod]
      public void Clears_RemovesAllItems() {
         var item = new Item(0);

         Collection.Add(item);
         Collection.Clear();

         CollectionAssert.AreEquivalent(
            new Item[0],
            Collection.ToArray()
         );

         GC.KeepAlive(item);
      }

      [TestMethod]
      public void Enumerate_WhenItemsDied_ReturnsOnlyLiveItems() {
         var item0 = new Item(0);
         var item1 = new Item(1);
         var item2 = new Item(2);

         Collection.Add(item0);
         Collection.Add(item1);
         Collection.Add(item2);

         item0 = null;
         item2 = null;
         GC.Collect();

         AssertEnumerationResult(item1);

         GC.KeepAlive(item1);
      }

      [TestMethod]
      public void Enumerate_WhenMoreThanPurgeThresholdItemsDied_ReturnsOnlyLiveItems() {
         var livingItem = new Item(-1);

         int dyingItemCount = WeakCollection<object>.PurgeThreshold + 15;
         for (int i = 0; i < dyingItemCount; i++) {
            Collection.Add(new Item(i));
         }

         Collection.Add(livingItem);
         GC.Collect();

         AssertEnumerationResult(livingItem);

         GC.KeepAlive(livingItem);
      }

      [TestMethod]
      public void Count_WhenItemsDied_ReturnsLiveCount() {
         var aliveItem = new Item(0);
         var dyingItem = new Item(1);

         Collection.Add(aliveItem);
         Collection.Add(dyingItem);

         dyingItem = null;
         GC.Collect();

         Assert.AreEqual(1, Collection.Count);

         GC.KeepAlive(aliveItem);
      }

      private void AssertEnumerationResult(params Item[] expected) {
         // Do not use ToList() because it uses CopyTo internally and we would not
         // test the actual IEnumerable implementation.
         var actual = new List<Item>();
         foreach (var item in Collection) {
            actual.Add(item);
         }

         CollectionAssert.AreEquivalent(expected, actual);
      }

      private class Item {
         private int _number;

         public Item(int number) {
            _number = number;
         }

         public override string ToString() {
            return _number.ToString();
         }
      }
   }
}