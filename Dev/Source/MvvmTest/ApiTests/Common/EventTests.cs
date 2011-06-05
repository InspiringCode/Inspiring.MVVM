namespace Inspiring.MvvmTest.ApiTests.Common {
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class EventTests {
      //   [TestMethod]
      //   public void Publish_OfAnEvent_ExecutesCorrectHandlers() {
      //      var mgr = CreateManager();

      //      var firstEventHandler = new TestHandler();
      //      var secondEventHandler = new TestHandler();

      //      mgr.Subscribe(TestEvents.FirstEvent, firstEventHandler, firstEventHandler.HandleEvent);
      //      mgr.Subscribe(TestEvents.SecondEvent, secondEventHandler, secondEventHandler.HandleEvent);

      //      var expectedPayload = new TestPayload();
      //      mgr.Publish(TestEvents.FirstEvent, expectedPayload);

      //      Assert.AreEqual(1, firstEventHandler.Invocations);
      //      Assert.AreEqual(0, secondEventHandler.Invocations);

      //      Assert.AreEqual(expectedPayload, firstEventHandler.LastPaylaod);
      //   }

      //   private EventAggregator CreateManager() {
      //      throw new NotImplementedException();
      //   }

      //   private class TestHandler {
      //      public int Invocations { get; set; }
      //      public TestPayload LastPaylaod { get; set; }

      //      public void HandleEvent(TestPayload payload) {
      //         Invocations++;
      //         LastPaylaod = payload;
      //      }
      //   }

      //   private class TestEvents {
      //      public static readonly Event<TestPayload> FirstEvent = new Event<TestPayload>();
      //      public static readonly Event<TestPayload> SecondEvent = new Event<TestPayload>();
      //   }

      //   private class TestPayload {
      //   }
      //}
   }
}