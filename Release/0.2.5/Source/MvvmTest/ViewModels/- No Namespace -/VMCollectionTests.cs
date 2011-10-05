﻿using System.Linq;
using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Inspiring.MvvmTest.ViewModels.__No_Namespace__ {
   [TestClass]
   public class VMCollectionTests : TestBase {

      [TestClass]
      public class VMCollection_ModificationBehavior_Tests : TestBase {
         private VMCollection<IViewModel> _collection;
         private Mock<IModificationCollectionBehavior<IViewModel>> _behaviorMock;
         private IBehaviorContext _context;
         private IViewModel _owner;

         [TestInitialize]
         public void Setup() {
            _context = Mock<IBehaviorContext>();

            var ownerMock = new Mock<IViewModel>();
            ownerMock.Setup(x => x.GetContext()).Returns(_context);
            _owner = ownerMock.Object;

            _behaviorMock = new Mock<IModificationCollectionBehavior<IViewModel>>();

            var behaviors = new BehaviorChain();
            behaviors.Successor = _behaviorMock.Object;

            _collection = new VMCollection<IViewModel>(behaviors, _owner);
         }

         [TestMethod]
         public void Add_BehaviorItemInsertedIsCalled() {
            var item = Mock<IViewModel>();
            _collection.Add(item);
            _behaviorMock.Verify(x => x.ItemInserted(_context, _collection, item, 0));
         }

         [TestMethod]
         public void Insert_BehaviorItemInsertedIsCalled() {
            var item = Mock<IViewModel>();
            _collection.Insert(0, item);
            _behaviorMock.Verify(x => x.ItemInserted(_context, _collection, item, 0));
         }

         [TestMethod]
         public void Remove_BehaviorItemRemovedIsCalled() {
            var item = Mock<IViewModel>();
            _collection.Add(item);

            _collection.Remove(item);

            _behaviorMock.Verify(x => x.ItemRemoved(_context, _collection, item, 0));
         }

         [TestMethod]
         public void SetItem_BehaviorItemSetIsCalled() {
            var previousItem = Mock<IViewModel>();
            var item = Mock<IViewModel>();
            _collection.Add(previousItem);

            _collection[0] = item;

            _behaviorMock.Verify(x => x.ItemSet(_context, _collection, previousItem, item, 0));
         }

         [TestMethod]
         public void Clear_BehaviorItemsClearedIsCalledAndItemRemovedIsNotCalled() {
            var item = Mock<IViewModel>();
            var previousItem = Mock<IViewModel>();
            _collection.Add(previousItem);

            _collection.Clear();

            _behaviorMock.Verify(x =>
               x.ItemsCleared(
                  _context,
                  _collection,
                  It.Is((IViewModel[] items) =>
                     items.Count() == 1 &&
                     items.First() == previousItem
                  )
               )
            );

            _behaviorMock.Verify(
               x => x.ItemRemoved(_context, _collection, It.IsAny<IViewModel>(), It.IsAny<int>()),
               Times.Never()
            );
         }

         [TestMethod]
         public void Move_BehaviorItemRemovedAndItemInsertedIsCalled() {
            var item0 = Mock<IViewModel>();
            var item1 = Mock<IViewModel>();

            _collection.Add(item0);
            _collection.Add(item1);

            _collection.Move(fromIndex: 0, toIndex: 1);

            _behaviorMock.Verify(x => x.ItemRemoved(_context, _collection, item0, 0));
            _behaviorMock.Verify(x => x.ItemInserted(_context, _collection, item0, 1));
         }
      }

      [TestMethod]
      public void Move_FromIndexOneToZero_Success() {
         var item0 = Mock<IViewModel>();
         var item1 = Mock<IViewModel>();

         var collection = new VMCollection<IViewModel>(new BehaviorChain(), Mock<IViewModel>());
         collection.Add(item0);
         collection.Add(item1);

         collection.Move(fromIndex: 1, toIndex: 0);

         CollectionAssert.AreEqual(new IViewModel[] { item1, item0 }, collection);
      }

      [TestMethod]
      public void Move_FromIndexZeroToOne_Success() {
         var item0 = Mock<IViewModel>();
         var item1 = Mock<IViewModel>();

         var collection = new VMCollection<IViewModel>(new BehaviorChain(), Mock<IViewModel>());
         collection.Add(item0);
         collection.Add(item1);

         collection.Move(fromIndex: 0, toIndex: 1);

         CollectionAssert.AreEqual(new IViewModel[] { item1, item0 }, collection);
      }

      [TestMethod]
      public void GetItemProperties_ReturnsPropertyDescriptorCollection() {
         var itemTypeDescriptorBehavior = new TypeDescriptorBehavior();

         var itemDescriptor = new VMDescriptor();
         itemDescriptor.Behaviors.Successor = itemTypeDescriptorBehavior;
         itemDescriptor.Behaviors.Initialize(itemDescriptor);

         var collectionBehaviors = new BehaviorChain();
         collectionBehaviors.Successor = new ItemDescriptorCollectionBehavior<IViewModel>(itemDescriptor);

         var collection = new VMCollection<IViewModel>(collectionBehaviors, Mock<IViewModel>());

         Assert.AreSame(itemTypeDescriptorBehavior.PropertyDescriptors, collection.GetItemProperties(null));
      }
   }
}