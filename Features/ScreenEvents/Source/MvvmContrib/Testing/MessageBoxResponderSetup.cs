//namespace Inspiring.Mvvm.Testing {
//   using System;
//   using Inspiring.Mvvm.Screens;

//   internal enum MessageBoxResponderType {
//      Info,
//      Warning,
//      Error,
//      YesNo,
//      YesNoCancel,
//      OkCancel
//   }

//   public class MessageBoxResponderSetup {
//      private readonly CustomDialogResult _result;
//      private readonly MessageBoxResponderType _responderType;

//      private StringExpectation _titleExpectation = new StringExpectation();
//      private StringExpectation _messageExpectation = new StringExpectation();

//      internal MessageBoxResponderSetup(
//         MessageBoxResponderType responderType,
//         CustomDialogResult result = CustomDialogResult.None
//      ) {
//         _responderType = responderType;
//         _result = result;
//      }

//      internal CustomDialogResult DialogResult {
//         get { return _result; }
//      }

//      public MessageBoxResponderSetup ExpectTitle(string exactTitle) {
//         _titleExpectation = new StringExpectation(exactTitle, MatchType.Exact);
//         return this;
//      }

//      public MessageBoxResponderSetup ExpectTitleExp(string titleRegex) {
//         _titleExpectation = new StringExpectation(titleRegex, MatchType.Regex);
//         return this;
//      }

//      public MessageBoxResponderSetup ExpectMessage(string exactMessage) {
//         _messageExpectation = new StringExpectation(exactMessage, MatchType.Exact);
//         return this;
//      }

//      public MessageBoxResponderSetup ExpectMessageExp(string messageRegex) {
//         _messageExpectation = new StringExpectation(messageRegex, MatchType.Regex);
//         return this;
//      }

//      internal void CheckInvocation(MessageBoxResponderType responderType, string message, string title) {
//         if (responderType != _responderType) {
//            throw new ArgumentException();
//         }

//         if (_titleExpectation.Matches(title)) {
//            throw new ArgumentException();
//         }

//         if (_messageExpectation.Matches(message)) {
//            throw new ArgumentException();
//         }
//      }
//   }
//}
