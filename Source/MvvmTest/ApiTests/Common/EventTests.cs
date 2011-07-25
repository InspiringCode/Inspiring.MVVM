namespace Inspiring.MvvmTest.ApiTests.Common {
   using Inspiring.Mvvm.Common;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System;
   using System.Text;

   [TestClass]
   public class EventTests {
      [TestMethod]
      public void Publish_OfAnEvent_ExecutesCorrectHandlers() {
         var agg = CreateAggregator();

         var firstEventHandler = new TestHandler();
         var secondEventHandler = new TestHandler();

         var sm = new EventSubscriptionManager(agg);
         sm.Subscribe(b => {
            b.On(TestEvents.FirstEvent).Execute(firstEventHandler.HandleEvent);
            b.On(TestEvents.SecondEvent).Execute(secondEventHandler.HandleEvent);
         });

         var expectedPayload = new TestPayload();
         agg.Publish(TestEvents.FirstEvent, expectedPayload);

         Assert.AreEqual(1, firstEventHandler.Invocations);
         Assert.AreEqual(0, secondEventHandler.Invocations);

         Assert.AreEqual(expectedPayload, firstEventHandler.LastPaylaod);

         GC.KeepAlive(sm);
      }

      [TestMethod]
      public void Publish_ExecutesOnlyHandlesWhoseConditionIsTrue() {
         var agg = CreateAggregator();

         var handlerConditionTrue = new TestHandler();
         var handlerConditionFalse = new TestHandler();

         TestPayload actualPayload = null;
         var sm = new EventSubscriptionManager(agg, b => {
            b.On(TestEvents.FirstEvent).When(p => false).Execute(handlerConditionFalse.HandleEvent);
            b.On(TestEvents.FirstEvent)
               .When(p => {
                  actualPayload = p;
                  return true;
               })
               .Execute(handlerConditionTrue.HandleEvent);
         });

         TestPayload expectedPayload = new TestPayload();
         agg.Publish(TestEvents.FirstEvent, expectedPayload);

         Assert.AreEqual(0, handlerConditionFalse.Invocations);
         Assert.AreEqual(1, handlerConditionTrue.Invocations);

         Assert.AreEqual(expectedPayload, actualPayload);

         GC.KeepAlive(sm);
      }

      [TestMethod]
      public void Publish_ExecutesHandlersInSpecifiedOrder() {
         var agg = CreateAggregator();
         var log = new StringBuilder();

         var firstHandler = new TestHandler(p => log.Append("First "));
         var beforeDefaultHandler = new TestHandler(p => log.Append("BeforeDefault "));
         var defaultHandler = new TestHandler(p => log.Append("Default "));
         var afterDefaultHandler = new TestHandler(p => log.Append("AfterDefault "));
         var lastHandler = new TestHandler(p => log.Append("Last "));


         var sm1 = new EventSubscriptionManager(agg, b => {
            b.On(TestEvents.FirstEvent).Execute(firstHandler.HandleEvent, ExecutionOrder.First);
            b.On(TestEvents.FirstEvent).Execute(defaultHandler.HandleEvent, ExecutionOrder.Default);
            b.On(TestEvents.FirstEvent).Execute(lastHandler.HandleEvent, ExecutionOrder.Last);
         });

         var sm2 = new EventSubscriptionManager(agg, b => {
            b.On(TestEvents.FirstEvent).Execute(beforeDefaultHandler.HandleEvent, ExecutionOrder.BeforeDefault);
            b.On(TestEvents.FirstEvent).Execute(afterDefaultHandler.HandleEvent, ExecutionOrder.AfterDefault);
         });

         agg.Publish(TestEvents.FirstEvent, new TestPayload());

         Assert.AreEqual(
            "First BeforeDefault Default AfterDefault Last ",
            log.ToString()
         );

         GC.KeepAlive(sm1);
         GC.KeepAlive(sm2);
      }

      [TestMethod]
      public void RemoveAllSubscriptions_HandlerAreNotExecutedAnymore() {
         var agg = CreateAggregator();

         var firstEventHandler = new TestHandler();
         var secondEventHandler = new TestHandler();

         var sm = new EventSubscriptionManager(agg, b => {
            b.On(TestEvents.FirstEvent).Execute(firstEventHandler.HandleEvent);
            b.On(TestEvents.SecondEvent).Execute(secondEventHandler.HandleEvent);
         });

         sm.RemoveAllSubscriptions();

         agg.Publish(TestEvents.FirstEvent, new TestPayload());
         agg.Publish(TestEvents.SecondEvent, new TestPayload());

         Assert.AreEqual(0, firstEventHandler.Invocations);
         Assert.AreEqual(0, secondEventHandler.Invocations);

         GC.KeepAlive(sm);
      }

      [TestMethod]
      public void RemoveSubscriptionTo_OnlyHandlerOfSpecifiedEventIsNotExecutedAnymore() {
         var agg = CreateAggregator();

         var firstEventHandler = new TestHandler();
         var secondEventHandler1 = new TestHandler();
         var secondEventHandler2 = new TestHandler();

         var sm = new EventSubscriptionManager(agg, b => {
            b.On(TestEvents.FirstEvent).Execute(firstEventHandler.HandleEvent);
            b.On(TestEvents.SecondEvent).Execute(secondEventHandler1.HandleEvent);
            b.On(TestEvents.SecondEvent).Execute(secondEventHandler2.HandleEvent);
         });

         sm.RemoveSubscriptionsTo(TestEvents.SecondEvent);

         agg.Publish(TestEvents.FirstEvent, new TestPayload());
         agg.Publish(TestEvents.SecondEvent, new TestPayload());

         Assert.AreEqual(1, firstEventHandler.Invocations);
         Assert.AreEqual(0, secondEventHandler1.Invocations);
         Assert.AreEqual(0, secondEventHandler2.Invocations);

         GC.KeepAlive(sm);
      }

      private EventAggregator CreateAggregator() {
         return new EventAggregator();
      }

      private class TestHandler {
         public TestHandler(Action<TestPayload> callback = null) {
            Callback = callback ?? (p => { });
         }

         public int Invocations { get; set; }
         public TestPayload LastPaylaod { get; set; }
         public Action<TestPayload> Callback { get; set; }

         public void HandleEvent(TestPayload payload) {
            Invocations++;
            LastPaylaod = payload;
            Callback(payload);
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