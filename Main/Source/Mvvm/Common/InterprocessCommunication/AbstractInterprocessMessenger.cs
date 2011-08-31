namespace Inspiring.Mvvm.Common.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Reflection;

   public abstract class AbstractInterprocessMessenger : IDisposable {
      private Action<object> _messageHandlers = delegate { };

      public AbstractInterprocessMessenger(string sharedIdentifier = null) {
         if (sharedIdentifier == null) {
            string entryTypeNamespace = Assembly
               .GetEntryAssembly()
               .EntryPoint
               .DeclaringType
               .FullName;

            SharedIdentifier = "IPC:" + entryTypeNamespace;
         }

         MessageQueue = new Queue<object>();
      }

      ~AbstractInterprocessMessenger() {
         Dispose(false);
      }

      public bool HasUndispatchedMessages {
         get { return MessageQueue.Any(); }
      }

      public abstract bool IsListening {
         get;
      }

      protected string SharedIdentifier {
         get;
         private set;
      }

      protected Queue<object> MessageQueue {
         get;
         private set;
      }

      /// <summary>
      ///   Queues a message which can later be sent using <see cref="DispatchMessages"/>.
      /// </summary>
      /// <exception cref="SerializationException">
      ///   <typeparamref name="TMessage"/> is not serializable.
      /// </exception>
      public void EnqueueMessage<TMessage>(TMessage message) {
         Contract.Requires<ArgumentNullException>(message != null);
         MessageQueue.Enqueue(message);
      }

      /// <summary>
      ///   Adds a handler action that is executed everytime a message of type
      ///   <typeparamref name="TMessage"/> is received.
      /// </summary>
      public void AddMessageReceiver<TMessage>(Action<TMessage> messageReceivedAction) {
         Contract.Requires<ArgumentNullException>(messageReceivedAction != null);

         _messageHandlers += (object message) => {
            if (message is TMessage) {
               messageReceivedAction((TMessage)message);
            }
         };
      }

      /// <summary>
      ///   Starts listening for message sent by other processes with the same
      ///   shared identifier.
      /// </summary>
      public void StartListening() {
         Contract.Requires<InvalidOperationException>(!IsListening);
         Contract.Ensures(IsListening);

         StartListeningCore();
      }

      /// <summary>
      ///   Stops listening for messages sent by other processes.
      /// </summary>
      public void StopListening() {
         Contract.Requires<InvalidOperationException>(IsListening);
         Contract.Ensures(!IsListening);

         StopListeningCore();
      }

      /// <summary>
      ///   Tries to send all queued messages to the passed in target processes.
      ///   Query <see cref="HasUndispatchedMessages"/> to determine if all messages
      ///   could be been dispatched successfully.
      /// </summary>
      public void DispatchMessages(DispatchTarget target) {
         while (HasUndispatchedMessages) {
            bool dispatchedSuccessfully = DispatchMessage(MessageQueue.Peek(), target);

            if (dispatchedSuccessfully) {
               MessageQueue.Dequeue();
            } else {
               break;
            }
         }
      }

      /// <inheritdoc />
      public void Dispose() {
         Dispose(true);
      }

      protected virtual void Dispose(bool disposing) {
         StopListeningCore();
      }

      protected abstract void StartListeningCore();

      protected abstract void StopListeningCore();

      protected abstract bool DispatchMessage(object message, DispatchTarget target);

      protected void InvokeMessageHandlers(object message) {
         _messageHandlers(message);
      }
   }
}
