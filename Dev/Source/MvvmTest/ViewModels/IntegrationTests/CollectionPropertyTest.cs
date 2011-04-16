namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionPropertyTest : TestBase {
      private TestVM _parent;
      private TestVMSource _parentSource;
      private ChildVMSource _firstChildSource;
      private ChildVMSource _secondChildSource;
      private ChildVMSource _thirdChildSource;

      [TestInitialize]
      public void Setup() {
         _firstChildSource = new ChildVMSource();
         _secondChildSource = new ChildVMSource();
         _thirdChildSource = new ChildVMSource();
         _parentSource = new TestVMSource();
         _parentSource.AddChild(_firstChildSource);
         _parentSource.AddChild(_secondChildSource);
         _parent = new TestVM();
         _parent.Source = _parentSource;
      }

      [TestMethod]
      public void CheckPrereqisites() {
         CompareCollections();
      }

      [TestMethod]
      public void AddItem() {
         ChildVM thirdChild = new ChildVM();
         thirdChild.InitializeFrom(_thirdChildSource);
         _parent.MappedCollectionAccessor.Add(thirdChild);
         CompareCollections();
      }

      [TestMethod]
      public void InsertItm() {
         ChildVM thirdChild = new ChildVM();
         thirdChild.InitializeFrom(_thirdChildSource);
         _parent.MappedCollectionAccessor.Insert(1, thirdChild);
         CompareCollections();
      }

      [TestMethod]
      public void RemoveItem() {
         _parent.MappedCollectionAccessor.RemoveAt(0);
         CompareCollections();
      }

      [TestMethod]
      public void Clearitems() {
         _parent.MappedCollectionAccessor.Clear();
         CompareCollections();
      }

      [TestMethod]
      public void SetTwoItems() {
         var item1 = _parent.MappedCollectionAccessor[0];
         var item2 = _parent.MappedCollectionAccessor[1];

         _parent.MappedCollectionAccessor[0] = item2;
         _parent.MappedCollectionAccessor[1] = item1;

         CompareCollections();
      }

      private void CompareCollections() {
         var actual = _parent
            .MappedCollectionAccessor
            .Select(x => x.Source).ToArray();

         var expected = _parentSource.ChildCollection.ToArray();

         CollectionAssert.AreEqual(expected, actual);
      }
   }
}