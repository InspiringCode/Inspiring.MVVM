//namespace Inspiring.Mvvm.Testing {
//   using System;
//   using System.Collections.Generic;
//   using Inspiring.Mvvm.Screens;

//   public class MessageBoxTestService : IMessageBoxService {
//      private Queue<MessageBoxResponderSetup> _responders = new Queue<MessageBoxResponderSetup>();

//      public MessageBoxResponderSetup EnqueueInfoResponder() {
//         return CreateAndEnqueueResponder(MessageBoxResponderType.Info);
//      }

//      public MessageBoxResponderSetup EnqueueWarningResponder() {
//         return CreateAndEnqueueResponder(MessageBoxResponderType.Warning);
//      }

//      public MessageBoxResponderSetup EnqueueErrorResponder() {
//         return CreateAndEnqueueResponder(MessageBoxResponderType.Error);
//      }

//      public MessageBoxResponderSetup EnqueueYesNoResponder(CustomDialogResult result) {
//         return CreateAndEnqueueResponder(MessageBoxResponderType.YesNo, result);
//      }

//      public MessageBoxResponderSetup EnqueueYesNoCancelResponder(CustomDialogResult result) {
//         return CreateAndEnqueueResponder(MessageBoxResponderType.YesNoCancel, result);
//      }

//      public MessageBoxResponderSetup EnqueueOkCancelResponder(CustomDialogResult result) {
//         return CreateAndEnqueueResponder(MessageBoxResponderType.OkCancel, result);
//      }

//      public void VerifyAllRespondersWereCalled() {
//         if (_responders.Count > 0) {
//            throw new ArgumentException();
//         }
//      }

//      void IMessageBoxService.Error(string message, string caption) {
//         CheckNextResponder(MessageBoxResponderType.Error, message, caption);
//      }

//      void IMessageBoxService.Info(string message, string caption) {
//         CheckNextResponder(MessageBoxResponderType.Info, message, caption);
//      }

//      void IMessageBoxService.Warning(string message, string caption) {
//         CheckNextResponder(MessageBoxResponderType.Warning, message, caption);
//      }

//      CustomDialogResult IMessageBoxService.YesNo(
//         string message,
//         string caption,
//         CustomDialogResult defaultResult,
//         CustomDialogIcon icon
//      ) {
//         return CheckNextResponder(MessageBoxResponderType.YesNo, message, caption);
//      }

//      CustomDialogResult IMessageBoxService.YesNoCancel(
//         string message,
//         string caption,
//         CustomDialogResult defaultResult,
//         CustomDialogIcon icon
//      ) {
//         return CheckNextResponder(MessageBoxResponderType.YesNoCancel, message, caption);
//      }

//      CustomDialogResult IMessageBoxService.OkCancel(
//         string message,
//         string caption,
//         CustomDialogResult defaultResult,
//         CustomDialogIcon icon
//      ) {
//         return CheckNextResponder(MessageBoxResponderType.OkCancel, message, caption);
//      }

//      private MessageBoxResponderSetup CreateAndEnqueueResponder(
//         MessageBoxResponderType type,
//         CustomDialogResult result = CustomDialogResult.None
//      ) {
//         MessageBoxResponderSetup responder = new MessageBoxResponderSetup(type, result);
//         _responders.Enqueue(responder);
//         return responder;
//      }

//      private CustomDialogResult CheckNextResponder(MessageBoxResponderType type, string message, string title) {
//         if (_responders.Count == 0) {
//            throw new ArgumentException();
//         }

//         MessageBoxResponderSetup responder = _responders.Dequeue();
//         responder.CheckInvocation(type, message, title);
//         return responder.DialogResult;
//      }
//   }
//}
