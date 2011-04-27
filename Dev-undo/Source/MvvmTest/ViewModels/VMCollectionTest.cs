﻿//namespace Inspiring.MvvmTest.ViewModels {
//   using System;
//   using System.Linq;
//   using Inspiring.Mvvm.ViewModels;
//   using Inspiring.Mvvm.ViewModels.Core;
//   using Microsoft.VisualStudio.TestTools.UnitTesting;
//   using Moq;
//   using Moq.Protected;

//   [TestClass]
//   public class VMCollectionTest : TestBase {
//      private VMCollection<ChildVM> _coll;
//      private ChildVM _firstElement;
//      private ChildVM _secondElement;
//      private ChildVM _thirdElement;

//      [TestInitialize]
//      public void Setup() {
//         _firstElement = new ChildVM();
//         _secondElement = new ChildVM();
//         _thirdElement = new ChildVM();
//      }

//      [TestMethod]
//      public void AddNewAndEndNew() {
//         CheckAddNew(() => _coll.EndNew(_coll.Count - 1));
//      }

//      [TestMethod]
//      public void AddNewAndAdd() {
//         CheckAddNew(() => _coll.Add(_thirdElement));
//      }

//      [TestMethod]
//      public void AddNewAndRemove() {
//         CheckAddNew(() => _coll.Remove(_firstElement));
//      }


//      [TestMethod]
//      public void AddNewAndInsert() {
//         CheckAddNew(() => _coll.Insert(0, _thirdElement));
//      }

//      [TestMethod]
//      public void AddNewAndClear() {
//         CheckAddNew(() => _coll.Clear());
//      }

//      [TestMethod]
//      public void AddItem() {
//         ExpectControllerInvocation(
//            () => _coll.Add(_thirdElement),
//            m => m.Setup(x => x.Insert(_thirdElement, 2)),
//            m => m.Verify(x => x.Insert(_thirdElement, 2), Times.Once())
//         );
//      }

//      [TestMethod]
//      public void InsertItem() {
//         ExpectControllerInvocation(
//            () => _coll.Insert(1, _thirdElement),
//            m => m.Setup(x => x.Insert(_thirdElement, 1)),
//            m => m.Verify(x => x.Insert(_thirdElement, 1), Times.Once())
//         );
//      }

//      [TestMethod]
//      public void RemoveItem() {
//         ExpectControllerInvocation(
//            () => _coll.Remove(_firstElement),
//            m => m.Setup(x => x.Remove(_firstElement)),
//            m => m.Verify(x => x.Remove(_firstElement), Times.Once())
//         );
//      }

//      [TestMethod]
//      public void SetItem() {
//         ExpectControllerInvocation(
//            () => _coll[1] = _thirdElement,
//            m => m.Setup(x => x.SetItem(_thirdElement, 1)),
//            m => m.Verify(x => x.SetItem(_thirdElement, 1), Times.Once())
//         );
//      }

//      [TestMethod]
//      public void Clear() {
//         ExpectControllerInvocation(
//            () => _coll.Clear(),
//            m => m.Setup(x => x.Clear()),
//            m => m.Verify(x => x.Clear(), Times.Once())
//         );
//      }

//      // [TestMethod] // TODO
//      public void OnValidating_OnValidated() {
//         var mock = new Mock<VMCollection<ChildVM>>(new TestVM(), ChildVM.Descriptor, null);
//         mock.CallBase = true;

//         var coll = mock.Object;

//         _firstElement.InitializeFrom(new ChildVMSource());
//         _secondElement.InitializeFrom(new ChildVMSource());

//         coll.Add(_firstElement);
//         coll.Add(_secondElement);

//         coll.First().MappeddMutableAccessor = "Test";
//         mock.Protected().Verify("OnItemValidating", Times.Exactly(4), ItExpr.IsAny<object>(), ItExpr.IsAny<ValidationEventArgs>());
//         mock.Protected().Verify("OnItemValidated", Times.Exactly(4), ItExpr.IsAny<object>(), ItExpr.IsAny<ValidationEventArgs>());
//      }

//      [TestMethod]
//      public void CollectionValidationBehaviorIsInvoked() {
//         int invocationCount = 0;

//         ValidationEventArgs args = new ValidationEventArgs(
//            new VMProperty<decimal>(),
//            42m,
//            new TestVM { LocalAccessor = 42m }
//         );

//         var behavior = new CollectionValidationBehavior<TestVM>();

//         var coll = new VMCollection<TestVM>(new TestVM(), TestVM.Descriptor, behavior);

//         behavior.Add((item, items, a) => {
//            invocationCount++;
//            Assert.AreSame(args, a);
//            Assert.AreSame(item, a.VM);
//            Assert.AreSame(items, coll);
//         });


//         coll.Invoke("OnItemValidating", coll, args);

//         Assert.AreEqual(1, invocationCount);
//      }

//      private void CheckAddNew(Action commitAction) {
//         var itemControllerMock = new Mock<IItemCreationController<ChildVM>>(MockBehavior.Strict);
//         _coll = new VMCollection<ChildVM>(new TestVM(), ChildVM.Descriptor);

//         _coll.Repopulate(
//            new ChildVM[] { _firstElement, _secondElement },
//            itemControllerMock.Object,
//            new Mock<ICollectionModificationController<ChildVM>>().Object
//         );

//         ChildVM addNewItem = new ChildVM();
//         itemControllerMock.Setup(x => x.AddNew()).Returns(addNewItem);

//         ChildVM addNew = _coll.AddNew();

//         Assert.IsNotNull(addNew);
//         CollectionAssert.AreEquivalent(
//            new ChildVM[] { _firstElement, _secondElement, addNewItem },
//            _coll
//         );
//         itemControllerMock.Verify(x => x.AddNew(), Times.Once());

//         itemControllerMock.Setup(x => x.EndNew(addNewItem));
//         commitAction();

//         itemControllerMock.Verify(x => x.EndNew(addNewItem), Times.Once());
//      }

//      private void ExpectControllerInvocation(
//         Action collectionAction,
//         Action<Mock<ICollectionModificationController<ChildVM>>> setupExpectation,
//         Action<Mock<ICollectionModificationController<ChildVM>>> verifyExpectation
//     ) {
//         var collectionControllerMock = new Mock<ICollectionModificationController<ChildVM>>(MockBehavior.Strict);
//         _coll = new VMCollection<ChildVM>(new TestVM(), ChildVM.Descriptor);

//         _coll.Repopulate(
//            new ChildVM[] { _firstElement, _secondElement },
//            new Mock<IItemCreationController<ChildVM>>().Object,
//            collectionControllerMock.Object
//         );

//         //collectionControllerMock.Setup(x => x.Insert(_firstElement, 0));
//         //_coll.Add(_firstElement);
//         //collectionControllerMock.Setup(x => x.Insert(_secondElement, 1));
//         //_coll.Add(_secondElement);

//         setupExpectation(collectionControllerMock);
//         collectionAction();
//         verifyExpectation(collectionControllerMock);
//      }
//   }
//}