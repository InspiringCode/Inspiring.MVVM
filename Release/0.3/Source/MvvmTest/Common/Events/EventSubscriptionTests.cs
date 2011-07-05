namespace Inspiring.MvvmTest.Common.Events {
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class EventSubscriptionTests {
      [TestMethod]
      public void EventSubscription_DoesNotKeepReceiverAlive() {
         Assert.Inconclusive("Events: TODO");
      }

      [TestMethod]
      public void EventHandlerDelegate_IsNotGarbageCollectedWhenReceiverIsStillAlive() {
         Assert.Inconclusive("Events: TODO");
      }

      [TestMethod]
      public void EventHandlerDelegate_IsGarbageCollectionWhenReceiverIsCollected() {
         Assert.Inconclusive("Events: TODO");
      }

      [TestMethod]
      public void EventHandlerDelegate_IsNotCollectedWhenItReferencesAStaticMethod() {
         Assert.Inconclusive("Events: TODO");
      }


   }
}