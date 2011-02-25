namespace Inspiring.MvvmTest.ViewModels.Core.Collections {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class CollectionChangeNotificationBehaviorTests : CollectionModificationBehaviorTestBase<IViewModel> {
      private Mock<IBehaviorContext> BehaviorContextMock { get; set; }

      [TestInitialize]
      public override void Setup() {
         base.Setup();

         BehaviorContextMock = new Mock<IBehaviorContext>();
         BehaviorContext = BehaviorContextMock.Object;

         Behavior = new ChangeNotifierCollectionBehavior<IViewModel>();
      }

      [TestMethod]
      public void ItemInserted_CallsNotifyChange() {
         var item = CreateItem();

         Behavior_ItemInserted(item);

         BehaviorContextMock.Verify(x =>
            x.NotifyChange(new ChangeArgs(ChangeType.AddedToCollection, item))
         );
      }

      [TestMethod]
      public void ItemRemoved_CallsNotifyChange() {
         var item = CreateItem();

         Behavior_ItemRemoved(item);

         BehaviorContextMock.Verify(x =>
            x.NotifyChange(new ChangeArgs(ChangeType.RemovedFromCollection, item))
         );
      }

      [TestMethod]
      public void ItemSet_CallsNotifyChange() {
         var item = CreateItem();

         Behavior_ItemSet(previousItem: CreateItem(), item: item);

         BehaviorContextMock.Verify(x =>
            x.NotifyChange(new ChangeArgs(ChangeType.AddedToCollection, item))
         );
      }

      [TestMethod]
      public void ItemSet_CallsNotifyChangeOnPreviousItem() {
         var previousItem = CreateItem();

         Behavior_ItemSet(previousItem, item: CreateItem());

         BehaviorContextMock.Verify(x =>
            x.NotifyChange(new ChangeArgs(ChangeType.RemovedFromCollection, previousItem))
         );
      }

      [TestMethod]
      public void ItemsCleared_CallsNotifyChange() {
         var previousItem = CreateItem();

         Behavior_ItemsCleared(previousItems: new IViewModel[] { previousItem });

         BehaviorContextMock.Verify(x =>
            x.NotifyChange(new ChangeArgs(ChangeType.RemovedFromCollection, previousItem))
         );
      }
   }
}
