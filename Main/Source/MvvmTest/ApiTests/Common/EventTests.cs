﻿namespace Inspiring.MvvmTest.ApiTests.Common {
   using Inspiring.Mvvm.Common;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class EventTests {
      [TestMethod]
      public void Publish_OfAnEvent_ExecutesCorrectHandlers() {
         Assert.Inconclusive("Still TODO: Events");

         var mgr = CreateManager();

         var firstEventHandler = new TestHandler();
         var secondEventHandler = new TestHandler();

         var sm = new EventSubscriptionManager(mgr, b => {
            b.On(TestEvents.FirstEvent).Execute(firstEventHandler.HandleEvent);
            b.On(TestEvents.SecondEvent).Execute(secondEventHandler.HandleEvent);
         });

         var expectedPayload = new TestPayload();
         mgr.Publish(TestEvents.FirstEvent, expectedPayload);

         Assert.AreEqual(1, firstEventHandler.Invocations);
         Assert.AreEqual(0, secondEventHandler.Invocations);

         Assert.AreEqual(expectedPayload, firstEventHandler.LastPaylaod);
      }

      private EventAggregator CreateManager() {
         return new EventAggregator();
      }

      private class TestHandler {
         public int Invocations { get; set; }
         public TestPayload LastPaylaod { get; set; }

         public void HandleEvent(TestPayload payload) {
            Invocations++;
            LastPaylaod = payload;
         }
      }

      private class TestEvents {
         public static readonly Event<TestPayload> FirstEvent = new Event<TestPayload>();
         public static readonly Event<TestPayload> SecondEvent = new Event<TestPayload>();
      }

      private class TestPayload {
      }
   }
}