namespace Inspiring.MvvmTest.Common.InterprocessCommunication {
   using System;
   using System.Runtime.CompilerServices;
   using System.Windows.Threading;
   using Inspiring.Mvvm.Common;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class InterprocessMessengerTests {
      private TestInterprocessMessenger Messenger { get; set; }

      [TestInitialize]
      public void Setup() {
         Messenger = new TestInterprocessMessenger();
      }

      [TestMethod]
      public void DispatchMessagesToLocalProcess_ExecutesRegisteredHandlers() {
         TestMessage actual = null;
         Messenger.AddMessageReceiver<TestMessage>(msg => actual = msg);

         var expected = new TestMessage("Expected");
         Messenger.EnqueueMessage(expected);

         Messenger.DispatchMessages(DispatchTarget.LocalProcess);

         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void TryDispatchMessagesToOtherProcess_DoesNotExecuteRegisteredHandlers() {
         bool wasExecuted = false;

         Messenger.DispatchToSelf = false;
         Messenger.AddMessageReceiver<TestMessage>(msg => wasExecuted = true);
         Messenger.EnqueueMessage(new TestMessage("Not handled message"));
         Messenger.DispatchMessages(DispatchTarget.FirstOtherProcess);

         Assert.IsFalse(wasExecuted);
         Assert.IsTrue(Messenger.HasUndispatchedMessages);
      }

      [TestMethod]
      public void TryDispatchMessagesToOtherProcess_SendsMessageToOtherWindows() {
         TestMessage actual = null;
         Messenger.AddMessageReceiver<TestMessage>(msg => actual = msg);

         var expected = new TestMessage("Expected Message");
         Messenger.EnqueueMessage(expected);
         Messenger.StartListening();
         Messenger.DispatchToSelf = true;

         Messenger.DispatchMessages(DispatchTarget.FirstOtherProcess);

         Assert.IsFalse(Messenger.HasUndispatchedMessages);
         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void TryDispatchMessagesToOtherProcess_SendsMessageToOtherWindows2() {
         TestMessage actual = null;
         Messenger.AddMessageReceiver<TestMessage>(msg => actual = msg);

         var expected = new TestMessage("Expected Message");
         Messenger.EnqueueMessage(expected);

         Messenger.StartListening();
         Messenger.DispatchToSelf = true;

         Messenger.DispatchMessages(DispatchTarget.FirstOtherProcess);

         Assert.IsFalse(Messenger.HasUndispatchedMessages);
         Assert.AreEqual(expected, actual);
      }

      [TestCleanup]
      public void Cleanup() {
         Messenger.Dispose();
         Dispatcher.CurrentDispatcher.InvokeShutdown();
      }

      private class TestInterprocessMessenger : InterprocessMessenger {
         public TestInterprocessMessenger()
            : base("IPC:TestIdentifier") {
         }

         public bool DispatchToSelf { get; set; }

         protected override bool IsOwnWindow(IntPtr windowHandle) {
            if (DispatchToSelf) {
               return false;
            }

            return base.IsOwnWindow(windowHandle);
         }
      }

      [Serializable]
      private class TestMessage {
         private readonly string _id;

         public TestMessage(string id) {
            _id = id;
         }

         public override int GetHashCode() {
            return RuntimeHelpers.GetHashCode(_id);
         }

         public override bool Equals(object obj) {
            var other = obj as TestMessage;
            return other != null && other._id == _id;
         }

         public override string ToString() {
            return _id;
         }
      }
   }
}