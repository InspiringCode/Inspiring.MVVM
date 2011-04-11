namespace Inspiring.MvvmTest.ViewModels.Core.Collections {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class CollectionChangeNotificationBehaviorTests : CollectionModificationBehaviorTestBase<IViewModel> {
      private Mock<IBehaviorContext> BehaviorContextMock { get; set; }
      private BehaviorContextStub ContextStub { get; set; }

      [TestInitialize]
      public override void Setup() {
         base.Setup();

         BehaviorContextMock = new Mock<IBehaviorContext>();
         BehaviorContext = ContextStub = new BehaviorContextStub();

         Behavior = new ChangeNotifierCollectionBehavior<IViewModel>();
      }

      [TestMethod]
      public void ItemInserted_CallsNotifyChange() {
         var item = CreateItem();

         Behavior_ItemInserted(item);

         AssertChangeArgs(
            new ChangeArgs(ChangeType.AddedToCollection, Collection, newItems: new[] { item })
         );
      }

      [TestMethod]
      public void ItemRemoved_CallsNotifyChange() {
         var item = CreateItem();

         Behavior_ItemRemoved(item);

         AssertChangeArgs(
            new ChangeArgs(ChangeType.RemovedFromCollection, Collection, oldItems: new[] { item })
         );
      }

      [TestMethod]
      public void ItemSet_CallsNotifyChangeForOldAndNewItem() {
         var oldItem = CreateItem();
         var item = CreateItem();

         Behavior_ItemSet(previousItem: oldItem, item: item);

         AssertChangeArgs(
            new ChangeArgs(ChangeType.RemovedFromCollection, Collection, oldItems: new[] { oldItem }),
            new ChangeArgs(ChangeType.AddedToCollection, Collection, newItems: new[] { item })
         );
      }

      [TestMethod]
      public void ItemsCleared_CallsNotifyChange() {
         var oldItems = new[] { CreateItem(), CreateItem() };

         Behavior_ItemsCleared(previousItems: oldItems);

         AssertChangeArgs(
            new ChangeArgs(ChangeType.RemovedFromCollection, Collection, oldItems: oldItems)
         );
      }

      [TestMethod]
      public void ReplaceItems_CallsNotifyChangeForOldAndNewItems() {
         var oldItems = new[] { CreateItem(), CreateItem() };
         var newItems = new[] { CreateItem() };

         Collection = CreateCollectionStub(newItems);

         Behavior_CollectionPopulated(oldItems);

         AssertChangeArgs(
            new ChangeArgs(ChangeType.RemovedFromCollection, Collection, oldItems: oldItems),
            new ChangeArgs(ChangeType.AddedToCollection, Collection, newItems: newItems)
         );
      }

      protected override IViewModel CreateAnonymousItem() {
         return CreateItem();
      }

      private void AssertChangeArgs(params ChangeArgs[] expected) {
         Assert.AreEqual(expected.Length, ContextStub.NotifyChangeArgsInvocations.Count);

         for (int i = 0; i < expected.Length; i++) {
            AssertAreEqual(expected[i], ContextStub.NotifyChangeArgsInvocations[i]);
         }
      }

      private static void AssertAreEqual(ChangeArgs expected, ChangeArgs actual) {
         Assert.IsNotNull(actual);

         Assert.AreEqual(expected.ChangeType, actual.ChangeType);

         CollectionAssert.AreEquivalent(expected.OldItems.ToArray(), actual.OldItems.ToArray());
         CollectionAssert.AreEquivalent(expected.NewItems.ToArray(), actual.NewItems.ToArray());

         Assert.AreEqual(expected.ChangedPath.Length, actual.ChangedPath.Length); // TODO: Better equal
      }

      private class BehaviorContextStub : IBehaviorContext {
         public BehaviorContextStub() {
            NotifyChangeArgsInvocations = new List<ChangeArgs>();
         }

         public List<ChangeArgs> NotifyChangeArgsInvocations { get; private set; }

         public IViewModel VM {
            get { throw new NotImplementedException(); }
         }

         public FieldValueHolder FieldValues {
            get { throw new NotImplementedException(); }
         }

         public IServiceLocator ServiceLocator {
            get { throw new NotImplementedException(); }
         }

         public void NotifyValidating(ValidationArgs args) {
         }

         public void NotifyChange(ChangeArgs args) {
            NotifyChangeArgsInvocations.Add(args);
         }
      }
   }
}
