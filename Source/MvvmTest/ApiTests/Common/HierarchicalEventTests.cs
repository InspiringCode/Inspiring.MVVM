namespace Inspiring.MvvmTest.ApiTests.Common {
   using System;
   using System.Linq;
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class HierarchicalEventTests {
      private List<EventSubscriptionManager> _referencesKeptAlive = new List<EventSubscriptionManager>();

      private EventAggregator Aggregator { get; set; }

      [TestInitialize]
      public void Setup() {
         Aggregator = new EventAggregator();
      }

      [TestMethod]
      public void Publish_ExecutesHandlersOfEntireHierarchyOrderedByHierarchySequenceAndThanByExecutionOrder() {
         string log = "";

         var subscriber1 = new TestSubscriber();
         var subscriber1a = new TestSubscriber();
         var subscriber1b = new TestSubscriber();

         var subscriber2 = new TestSubscriber();

         subscriber1.Children.Add(subscriber1a);
         subscriber1.Children.Add(subscriber1b);

         var ev = new TestEvent();

         SubscribeTo(ev, subscriber1a, args => log += "1a ", ExecutionOrder.Default);
         SubscribeTo(ev, subscriber1a, args => log += "1a_After ", ExecutionOrder.AfterDefault);
         SubscribeTo(ev, subscriber1a, args => log += "1a_Before ", ExecutionOrder.BeforeDefault);
         SubscribeTo(ev, subscriber1, args => log += "1 ", ExecutionOrder.Default);
         SubscribeTo(ev, subscriber1, args => log += "1_After ", ExecutionOrder.AfterDefault);
         SubscribeTo(ev, subscriber1, args => log += "1_Before ", ExecutionOrder.BeforeDefault);
         SubscribeTo(ev, subscriber1b, args => log += "1b ", ExecutionOrder.Default);

         SubscribeTo(ev, subscriber2, args => log += "2 ", ExecutionOrder.Default);

         Aggregator.Publish(ev, new TestEventArgs(subscriber1));

         Assert.AreEqual("1_Before 1 1_After 1a_Before 1a 1a_After 1b ", log);
      }

      private void SubscribeTo(
         TestEvent @event,
         TestSubscriber target,
         Action<TestEventArgs> handler,
         ExecutionOrder order
      ) {
         var sm = new EventSubscriptionManager(Aggregator);
         sm.Subscribe(b => {
            b.On(@event, target)
               .When(args => true)
               .Execute(handler, order);
         });
      }


      private class TestEvent : HierarchicalEvent<TestSubscriber, TestEventArgs> {
         protected override IEnumerable<TestSubscriber> GetHierarchyNodes(TestSubscriber root) {
            return new[] { root }.Concat(root.Children);
         }
      }

      private class TestEventArgs : HierarchicalEventArgs<TestSubscriber> {
         public TestEventArgs(TestSubscriber target)
            : base(target) {
         }
      }

      private class TestSubscriber {
         public TestSubscriber() {
            Children = new List<TestSubscriber>();
         }

         public List<TestSubscriber> Children { get; private set; }

      }
   }
}