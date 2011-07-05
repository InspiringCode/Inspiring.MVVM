namespace Inspiring.MvvmTest.ViewModels.Core.Properties.CollectionProperty {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   // TODO: Make this test more readable, less obscure and less fragile!
   [TestClass]
   public class CollectionSynchronizerBehaviorTests : TestBase {
      private static IValueAccessorBehavior<IEnumerable<ItemSource>> CreateSourceCollectionAccessor(
         IEnumerable<ItemSource> sourceItems
      ) {
         return ValueAccessorStub.WithInitialValue(sourceItems);
      }

      private static ItemVM CreateItemWithSource(ItemSource source) {
         return new ItemVM { Source = source };
      }

      private static ItemVM CreateItemWithSourceDescription(string description) {
         return new ItemVM { Source = new ItemSource(description) };
      }

      //
      //   I E N U M E R A B L E   S O U R C E   T E S T S 
      //

      [TestClass]
      public class IEnumerableSource : CollectionChangeHandlerBehaviorTestBase<ItemVM> {
         private Mock<IEnumerable<ItemSource>> SourceEnumerable { get; set; }

         [TestInitialize]
         public void Setup() {
            SourceEnumerable = new Mock<IEnumerable<ItemSource>>(MockBehavior.Strict);
            Behavior = new SynchronizerCollectionBehavior<ItemVM, ItemSource>();

            SetupFixture(
               Behavior,
               CreateSourceCollectionAccessor(SourceEnumerable.Object)
            );
         }

         [TestMethod]
         public void ItemInserted_DoesNothing() {
            var insertedVM = CreateItemWithSourceDescription("Inserted source");
            Collection = CreateCollection(insertedVM);
            HandleItemInserted(insertedVM);
         }

         [TestMethod]
         public void ItemRemoved_DoesNothing() {
            var removedVM = CreateItemWithSourceDescription("Removed source");
            Collection = CreateCollection();
            HandleItemRemoved(removedVM);
         }

         [TestMethod]
         public void ItemSet_DoesNothing() {
            var oldVM = CreateItemWithSourceDescription("Old source");
            var newVM = CreateItemWithSourceDescription("New source");
            Collection = CreateCollection(newVM);

            HandleItemSet(previousItem: oldVM, item: newVM);
         }

         [TestMethod]
         public void CollectionCleared_DoesNothing() {
            var oldVM = CreateItemWithSourceDescription("Old source");
            Collection = CreateCollection();

            HandleCollectionCleared(previousItems: new ItemVM[] { oldVM });
         }

         [TestMethod]
         public void CollectionPopulated_DoesNothing() {
            var oldVM = CreateItemWithSourceDescription("Old source");
            var newVM = CreateItemWithSourceDescription("New source");
            Collection = CreateCollection(newVM);

            HandleCollectionPopulated(previousItems: new[] { oldVM });
         }
      }


      //
      //   I C O L L E C T I O N   S O U R C E   T E S T S
      //

      [TestClass]
      public class ICollectionSource : CollectionChangeHandlerBehaviorTestBase<ItemVM> {
         private Mock<CollectionMock<ItemSource>> _sourceCollection;
         private ValueAccessorStub<IEnumerable<ItemSource>> _sourceCollectionAccessor;

         private Mock<CollectionMock<ItemSource>> SourceCollection {
            get { return _sourceCollection; }
            set {
               _sourceCollection = value;
               _sourceCollectionAccessor.Value = value.Object;
            }
         }

         [TestInitialize]
         public void Setup() {
            _sourceCollectionAccessor = new ValueAccessorStub<IEnumerable<ItemSource>>();
            Behavior = new SynchronizerCollectionBehavior<ItemVM, ItemSource>();

            SetupFixture(
               Behavior,
               _sourceCollectionAccessor
            );
         }

         [TestMethod]
         public void ItemInserted_InsertsItemIntoSourceCollection() {
            var insertedVM = CreateItemWithSourceDescription("Inserted source");
            Collection = CreateCollection(insertedVM);
            SourceCollection = CreateSourceCollection();

            HandleItemInserted(insertedVM);
            SourceCollection.Verify(x => x.Add(insertedVM.Source), Times.Once());
         }

         [TestMethod]
         public void ItemRemoved_RemovesItemFromSourceCollection() {
            var removedVM = CreateItemWithSourceDescription("Removed source");
            Collection = CreateCollection();
            SourceCollection = CreateSourceCollection(removedVM.Source);

            HandleItemRemoved(removedVM);
            SourceCollection.Verify(x => x.Remove(removedVM.Source), Times.Once());
         }

         [TestMethod]
         public void ItemSet_RemovesOldItemAndAddsNewItemToSourceCollection() {
            var oldVM = CreateItemWithSourceDescription("Old source");
            var newVM = CreateItemWithSourceDescription("New source");
            Collection = CreateCollection(newVM);
            SourceCollection = CreateSourceCollection(oldVM.Source);

            HandleItemSet(previousItem: oldVM, item: newVM);
            SourceCollection.Verify(x => x.Remove(oldVM.Source), Times.Once());
            SourceCollection.Verify(x => x.Add(newVM.Source), Times.Once());
         }

         [TestMethod]
         public void ItemsCleared_CallsClearOnSourceCollection() {
            var oldVM = CreateItemWithSourceDescription("Old source");
            Collection = CreateCollection();
            SourceCollection = CreateSourceCollection(oldVM.Source);


            HandleCollectionCleared(previousItems: new ItemVM[] { oldVM });
            SourceCollection.Verify(x => x.Clear(), Times.Once());
         }

         [TestMethod]
         public void CollectionPopulated_DoesNothing() {
            var oldVM = CreateItemWithSourceDescription("Old source");
            var newVM = CreateItemWithSourceDescription("New source");

            Collection = CreateCollection(newVM);
            SourceCollection = CreateStrictSourceCollection(oldVM.Source);

            HandleCollectionPopulated(previousItems: new[] { oldVM });
            SourceCollection.VerifyAll();
         }

         private static Mock<CollectionMock<ItemSource>> CreateSourceCollection(
            params ItemSource[] initialItems
         ) {
            return new Mock<CollectionMock<ItemSource>>(new[] { initialItems }) {
               CallBase = true
            };
         }

         private static Mock<CollectionMock<ItemSource>> CreateStrictSourceCollection(
           params ItemSource[] initialItems
         ) {
            return new Mock<CollectionMock<ItemSource>>(MockBehavior.Strict, new[] { initialItems }) {
               CallBase = true
            };
         }
      }


      //
      //   I L I S T   S O U R C E   T E S T S 
      //

      [TestClass]
      public class IListSource : CollectionChangeHandlerBehaviorTestBase<ItemVM> {
         private Mock<ListMock<ItemSource>> _sourceList;
         private ValueAccessorStub<IEnumerable<ItemSource>> _sourceCollectionAccessor;

         private Mock<ListMock<ItemSource>> SourceList {
            get { return _sourceList; }
            set {
               _sourceList = value;
               _sourceCollectionAccessor.Value = value.Object;
            }
         }

         [TestInitialize]
         public void Setup() {
            _sourceCollectionAccessor = new ValueAccessorStub<IEnumerable<ItemSource>>();
            Behavior = new SynchronizerCollectionBehavior<ItemVM, ItemSource>();

            SetupFixture(
               Behavior,
               _sourceCollectionAccessor
            );
         }

         [TestMethod]
         public void ItemInserted_InsertsItemIntoSourceCollection() {
            var insertedVM = CreateItemWithSourceDescription("Inserted source");
            Collection = CreateCollection(insertedVM);
            SourceList = CreateSourceCollection();

            HandleItemInserted(insertedVM);
            SourceList.Verify(x => x.Insert(0, insertedVM.Source), Times.Once());
         }

         [TestMethod]
         public void ItemRemoved_RemovesItemFromSourceCollection() {
            var removedVM = CreateItemWithSourceDescription("Removed source");
            Collection = CreateCollection();
            SourceList = CreateSourceCollection(removedVM.Source);

            HandleItemRemoved(removedVM);
            SourceList.Verify(x => x.RemoveAt(0), Times.Once());
         }

         [TestMethod]
         public void ItemSet_RemovesOldItemAndAddsNewItemToSourceCollection() {
            var oldVM = CreateItemWithSourceDescription("Old source");
            var newVM = CreateItemWithSourceDescription("New source");
            Collection = CreateCollection(newVM);
            SourceList = CreateSourceCollection(oldVM.Source);

            HandleItemSet(previousItem: oldVM, item: newVM);
            SourceList.VerifySet(x => x[0] = newVM.Source);
         }

         [TestMethod]
         public void ItemsCleared_CallsClearOnSourceCollection() {
            var oldVM = CreateItemWithSourceDescription("Old source");
            Collection = CreateCollection();
            SourceList = CreateSourceCollection(oldVM.Source);


            HandleCollectionCleared(previousItems: new ItemVM[] { oldVM });
            SourceList.Verify(x => x.Clear(), Times.Once());
         }

         [TestMethod]
         public void CollectionPopulated_DoesNothing() {
            var oldVM = CreateItemWithSourceDescription("Old source");
            var newVM = CreateItemWithSourceDescription("New source");

            Collection = CreateCollection(newVM);
            SourceList = CreateStrictSourceCollection(oldVM.Source);

            HandleCollectionPopulated(previousItems: new[] { oldVM });
            SourceList.VerifyAll();
         }

         private static Mock<ListMock<ItemSource>> CreateSourceCollection(
            params ItemSource[] initialItems
         ) {
            return new Mock<ListMock<ItemSource>>(new[] { initialItems }) {
               CallBase = true
            };
         }

         private static Mock<ListMock<ItemSource>> CreateStrictSourceCollection(
           params ItemSource[] initialItems
         ) {
            return new Mock<ListMock<ItemSource>>(MockBehavior.Strict, new[] { initialItems }) {
               CallBase = true
            };
         }
      }


      //
      //  M O C K   C L A S S E S
      //

      public class ItemSource {
         public ItemSource(string description = null) {
            Description = description;
         }

         public string Description { get; set; }

         public override string ToString() {
            return Description ?? base.ToString();
         }
      }

      public class ItemVM : ViewModelStub, IHasSourceObject<ItemSource> {
         public ItemSource Source { get; set; }

         public override string ToString() {
            return String.Format("VM for {0}", Source.ToString());
         }
      }

      public class CollectionMock<T> : ICollection<T> {
         protected IList<T> _list;

         public CollectionMock(T[] initialItems) {
            _list = initialItems.ToList();
         }

         public CollectionMock() {
            _list = new List<T>();
         }

         public int Count {
            get { return _list.Count; }
         }

         public bool IsReadOnly {
            get { return _list.IsReadOnly; }
         }

         public virtual void Add(T item) {
            _list.Add(item);
         }

         public virtual bool Remove(T item) {
            return _list.Remove(item);
         }

         public virtual void Clear() {
            _list.Clear();
         }

         public bool Contains(T item) {
            return _list.Contains(item);
         }

         public void CopyTo(T[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
         }

         public IEnumerator<T> GetEnumerator() {
            return _list.GetEnumerator();
         }

         IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
         }
      }

      public class ListMock<T> : CollectionMock<T>, IList<T> {
         public ListMock() {
         }

         public ListMock(T[] initialItems)
            : base(initialItems) {
         }

         public int IndexOf(T item) {
            return _list.IndexOf(item);
         }

         public virtual void Insert(int index, T item) {
            _list.Insert(index, item);
         }

         public virtual void RemoveAt(int index) {
            _list.RemoveAt(index);
         }

         public virtual T this[int index] {
            get { return _list[index]; }
            set { _list[index] = value; }
         }
      }
   }
}
