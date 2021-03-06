﻿namespace Inspiring.MvvmTest.ViewModels.Core.Properties.CollectionProperty {
   using System.Collections.Generic;
   using System.Linq;
   using System.Text;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class WrapperCollectionAccessorBehaviorTests : CollectionAccessorBehaviorFixture {
      private const string ReplaceItemsLog = "ReplaceItems ";
      private WrapperCollectionAccessorBehavior<ItemVM, ItemSource> Behavior { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new WrapperCollectionAccessorBehavior<ItemVM, ItemSource>(false);
         Context = CreateContext(Behavior);
      }

      [TestMethod]
      public void GetValue_Initially_CreatesAndReturnsPopulatedInstance() {
         Next.SourceCollectionToReturn = new[] { new ItemSource() };

         var actual = Behavior.GetValue(Context);

         CollectionAssert.AreEqual(
            Next.SourceCollectionToReturn.ToArray(),
            actual.Select(x => x.Source).ToArray()
         );
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
      }

      [TestMethod]
      public void Refresh_ReusesPreviousInstancesAndRefreshesThem() {
         var sharedSourceItem = new ItemSource("Shared item");

         var previousSourceItems = new[] { 
            new ItemSource("Old item"),
            sharedSourceItem
         };

         Next.SourceCollectionToReturn = previousSourceItems;

         var previousVMs = Behavior
            .GetValue(Context)
            .ToArray();

         var newSourceItems = new[] {
            sharedSourceItem,
            new ItemSource("New item 1"),
            new ItemSource("New item 2")
         };

         Next.SourceCollectionToReturn = newSourceItems;
         InvokeRefresh();

         var newVMs = Behavior.GetValue(Context);

         CollectionAssert.AreEqual(
            newSourceItems,
            newVMs.Select(x => x.Source).ToArray()
         );

         var expectedSharedItemVM = previousVMs[1];
         var actualSharedItemVM = newVMs[0];

         Assert.AreEqual(expectedSharedItemVM, actualSharedItemVM);
         Assert.AreEqual(1, actualSharedItemVM.RefreshInvocations);

         var unsharedItemVMs = newVMs.Skip(1).ToArray();

         Assert.IsTrue(unsharedItemVMs.All(x => x.RefreshInvocations == 0));
      }

      [TestMethod]
      public void Refresh_FirstReplacesItemsAndThenRefreshesReusedItems() {
         var sharedSourceItem = new ItemSource("Shared item");

         StringBuilder actionLog = new StringBuilder();
         Next.ActionLog = actionLog;
         Next.CollectionToReturn = new LoggingCollectionStub(actionLog);

         Next.SourceCollectionToReturn = new[] { new ItemSource("Old item"), sharedSourceItem };
         Behavior.GetValue(Context);
         Next.SourceCollectionToReturn = new[] { sharedSourceItem, new ItemSource("New item") };

         InvokeRefresh();

         Assert.AreEqual(ReplaceItemsLog + ReplaceItemsLog + RefreshItemLog, actionLog.ToString());
      }

      [TestMethod]
      public void Refresh_CallsNextBehavior() {
         InvokeRefresh();
         Assert.AreEqual(1, Next.RefreshInvocations);
      }

      private void InvokeRefresh() {
         Behavior.Refresh(Context, new RefreshOptions());
      }

      private class LoggingCollectionStub : VMCollectionStub<ItemVM> {
         private StringBuilder _log;

         public LoggingCollectionStub(StringBuilder log)
            : base(ViewModelStub.Build(), PropertyStub.Build()) {
            _log = log;
         }
         public override void ReplaceItems(IEnumerable<ItemVM> newItems, IChangeReason reason) {
            _log.Append(ReplaceItemsLog);
            base.ReplaceItems(newItems, reason);
         }
      }
   }
}