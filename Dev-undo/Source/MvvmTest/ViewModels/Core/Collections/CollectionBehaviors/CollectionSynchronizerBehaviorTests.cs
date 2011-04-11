namespace Inspiring.MvvmTest.ViewModels.Core.Collections {
   using System.Collections;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;
   using Moq.Protected;

   // TODO: Make this test more readable, less obscure and less fragile!
   [TestClass]
   public class CollectionSynchronizerBehaviorTests : VMCollectionTestBase {
      [TestClass]
      public class IEnumerableSource : CollectionModificationBehaviorTestBase<ItemVM> {
         private Mock<IEnumerable<ItemSource>> CollectionSourceMock { get; set; }

         [TestInitialize]
         public override void Setup() {
            base.Setup();
            CollectionSourceMock = new Mock<IEnumerable<ItemSource>>();
            Behavior = new SynchronizerCollectionBehavior<ItemVM, ItemSource>();
            Behavior.Successor = CreateCollectionSourceBehavior(CollectionSourceMock.Object);
         }

         [TestMethod]
         public void ItemInserted_InsertsItem() {
            var itemSource = new ItemSource();
            Behavior_ItemInserted(CreateItem(itemSource));
            // Does nothing
         }

         [TestMethod]
         public void ItemRemoved_RemovesItem() {
            var itemSource = new ItemSource();
            Behavior_ItemRemoved(CreateItem(itemSource));
            // Does nothing
         }

         [TestMethod]
         public void ItemSet_SetsItem() {
            var itemSource = new ItemSource();
            var previousItemSource = new ItemSource();

            Behavior_ItemSet(CreateItem(previousItemSource), CreateItem(itemSource));

            // Does nothing
         }

         [TestMethod]
         public void ItemsCleared() {
            var previousItemSource = new ItemSource();
            Behavior_ItemsCleared(new ItemVM[] { CreateItem(previousItemSource) });
            // Does nothing
         }

         private static ItemVM CreateItem(ItemSource source) {
            var vm = new ItemVM();
            vm.InitializeFrom(source);
            return vm;
         }

         protected override ItemVM CreateAnonymousItem() {
            return new ItemVM();
         }
      }

      //[TestClass]
      //public abstract class SynchronizerTestBase : CollectionModificationBehaviorTestBase<ItemVM> {
      //   [TestInitialize]
      //   public override void Setup() {
      //      base.Setup();
      //      Behavior = new CollectionSynchronizerBehavior<ItemVM, ItemSource>();
      //      Behavior.Successor = CreateCollectionSourceBehavior(CollectionSource);
      //   }

      //   protected IEnumerable<ItemSource> CollectionSource { get; set; }

      //   [TestMethod]
      //   public void ItemInserted() {
      //      var itemSource = new ItemSource();
      //      Behavior_ItemInserted(CreateItem(itemSource));
      //      VerifyItemInserted(itemSource, index: 0);
      //   }

      //   [TestMethod]
      //   public void ItemRemoved() {
      //      var itemSource = new ItemSource();
      //      Behavior_ItemRemoved(CreateItem(itemSource));
      //      VerifyItemRemoved(itemSource);
      //   }

      //   [TestMethod]
      //   public void ItemSet() {
      //      var previousItemSource = new ItemSource();
      //      var itemSource = new ItemSource();

      //      Behavior_ItemSet(CreateItem(previousItemSource), CreateItem(itemSource));

      //      VerifyItemRemoved(previousItemSource);
      //      VerifyItemInserted(itemSource, 0);
      //   }

      //   [TestMethod]
      //   public void ItemsCleared() {
      //      var previousItemSource = new ItemSource();
      //      Behavior_ItemsCleared(new ItemVM[] { CreateItem(previousItemSource) });
      //      VerifyItemsCleared();
      //   }

      //   protected abstract void VerifyItemInserted(ItemSource itemSource, int index);

      //   protected abstract void VerifyItemRemoved(ItemSource itemSource);

      //   protected abstract void VerifyItemSet(ItemSource itemSource, int index);

      //   protected abstract void VerifyItemsCleared();

      //   private static ItemVM CreateItem(ItemSource source) {
      //      var vm = new ItemVM();
      //      vm.InitializeFrom(source);
      //      return vm;
      //   }
      //}

      [TestClass]
      public class IListSource : CollectionModificationBehaviorTestBase<ItemVM> {
         private Mock<ListMock> CollectionSourceMock { get; set; }

         [TestInitialize]
         public override void Setup() {
            base.Setup();
            CollectionSourceMock = new Mock<ListMock>() { CallBase = true };
            Behavior = new SynchronizerCollectionBehavior<ItemVM, ItemSource>();
            Behavior.Successor = CreateCollectionSourceBehavior(CollectionSourceMock.Object);
         }

         [TestMethod]
         public void ItemInserted_InsertsItem() {
            var itemSource = new ItemSource();
            Collection = CreateCollectionStub(itemCount: 1);

            Behavior_ItemInserted(CreateItem(itemSource));

            CollectionSourceMock.Protected().Verify("InsertItem", Times.Once(), 0, itemSource);
         }

         [TestMethod]
         public void ItemRemoved_RemovesItem() {
            var itemSource = new ItemSource();
            CollectionSourceMock.Object.Add(itemSource);
            Collection = CreateCollectionStub(itemCount: 0);

            Behavior_ItemRemoved(CreateItem(itemSource));

            CollectionSourceMock.Protected().Verify("RemoveItem", Times.Once(), 0);
         }

         [TestMethod]
         public void ItemSet_SetsItem() {
            var itemSource = new ItemSource();
            var previousItemSource = new ItemSource();
            CollectionSourceMock.Object.Add(previousItemSource);
            Collection = CreateCollectionStub(itemCount: 1);

            Behavior_ItemSet(CreateItem(previousItemSource), CreateItem(itemSource));

            CollectionSourceMock.Protected().Verify("SetItem", Times.Once(), 0, itemSource);
         }

         [TestMethod]
         public void ItemsCleared() {
            var previousItemSource = new ItemSource();
            CollectionSourceMock.Object.Add(previousItemSource);
            Collection = CreateCollectionStub(itemCount: 1);

            Behavior_ItemsCleared(new ItemVM[] { CreateItem(previousItemSource) });

            CollectionSourceMock.Protected().Verify("ClearItems", Times.Once());
         }

         private static ItemVM CreateItem(ItemSource source) {
            var vm = new ItemVM();
            vm.InitializeFrom(source);
            return vm;
         }

         public class ListMock : Collection<ItemSource> {
         }

         protected override ItemVM CreateAnonymousItem() {
            return new ItemVM();
         }
      }

      [TestClass]
      public class ICollectionSource : CollectionModificationBehaviorTestBase<ItemVM> {
         private Mock<ListMock> CollectionSourceMock { get; set; }

         [TestInitialize]
         public override void Setup() {
            base.Setup();
            CollectionSourceMock = new Mock<ListMock>() { CallBase = true };
            Behavior = new SynchronizerCollectionBehavior<ItemVM, ItemSource>();
            Behavior.Successor = CreateCollectionSourceBehavior(new CollectionAdapter(CollectionSourceMock.Object));
         }

         [TestMethod]
         public void ItemInserted_InsertsItem() {
            var itemSource = new ItemSource();
            Collection = CreateCollectionStub(itemCount: 1);

            Behavior_ItemInserted(CreateItem(itemSource));

            CollectionSourceMock.Protected().Verify("InsertItem", Times.Once(), 0, itemSource);
         }

         [TestMethod]
         public void ItemRemoved_RemovesItem() {
            var itemSource = new ItemSource();
            CollectionSourceMock.Object.Add(itemSource);
            Collection = CreateCollectionStub(itemCount: 0);

            Behavior_ItemRemoved(CreateItem(itemSource));

            CollectionSourceMock.Protected().Verify("RemoveItem", Times.Once(), 0);
         }

         [TestMethod]
         public void ItemSet_SetsItem() {
            var itemSource = new ItemSource();
            var previousItemSource = new ItemSource();
            CollectionSourceMock.Object.Add(previousItemSource);
            Collection = CreateCollectionStub(itemCount: 1);

            Behavior_ItemSet(CreateItem(previousItemSource), CreateItem(itemSource));

            CollectionSourceMock.Protected().Verify("RemoveItem", Times.Once(), 0);
            CollectionSourceMock.Protected().Verify("InsertItem", Times.Once(), 0, itemSource);
         }

         [TestMethod]
         public void ItemsCleared() {
            var previousItemSource = new ItemSource();
            CollectionSourceMock.Object.Add(previousItemSource);
            Collection = CreateCollectionStub(itemCount: 1);

            Behavior_ItemsCleared(new ItemVM[] { CreateItem(previousItemSource) });

            CollectionSourceMock.Protected().Verify("ClearItems", Times.Once());
         }

         private static ItemVM CreateItem(ItemSource source) {
            var vm = new ItemVM();
            vm.InitializeFrom(source);
            return vm;
         }

         public class ListMock : Collection<ItemSource> {
         }

         private class CollectionAdapter : ICollection<ItemSource> {
            private IList<ItemSource> _list;

            public CollectionAdapter(IList<ItemSource> list) {
               _list = list;
            }

            public void Add(ItemSource item) {
               _list.Add(item);
            }

            public void Clear() {
               _list.Clear();
            }

            public bool Contains(ItemSource item) {
               return _list.Contains(item);
            }

            public void CopyTo(ItemSource[] array, int arrayIndex) {
               _list.CopyTo(array, arrayIndex);
            }

            public int Count {
               get { return _list.Count; }
            }

            public bool IsReadOnly {
               get { return _list.IsReadOnly; }
            }

            public bool Remove(ItemSource item) {
               return _list.Remove(item);
            }

            public IEnumerator<ItemSource> GetEnumerator() {
               return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
               return _list.GetEnumerator();
            }
         }

         protected override ItemVM CreateAnonymousItem() {
            return new ItemVM();
         }
      }
   }
}
