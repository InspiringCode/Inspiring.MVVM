namespace Inspiring.Mvvm.Testing {
   using Inspiring.Mvvm.Screens;

   internal enum DialogServiceMethod {
      Info,
      Warning,
      Error,
      YesNo,
      YesNoCancel,
      OkCancel,
      ShowOpenFileDialog,
      ShowFolderBrowseDialog,
      OpenDialog
   }

   public class MessageBoxResponderSetup : ResponderBase {
      internal MessageBoxResponderSetup(
         DialogServiceMethod method,
         CustomDialogResult result = CustomDialogResult.None
      )
         : base(method) {
         DialogResult = result;
      }

      internal CustomDialogResult DialogResult {
         get;
         private set;
      }

      public MessageBoxResponderSetup ExpectCaption(string exactCaption) {
         ExpectedInvocation.Caption.SetValue(exactCaption, MatchType.Exact);
         return this;
      }

      public MessageBoxResponderSetup ExpectCaptionExp(string captionRegex) {
         ExpectedInvocation.Caption.SetValue(captionRegex, MatchType.Regex);
         return this;
      }

      public MessageBoxResponderSetup ExpectMessage(string exactMessage) {
         ExpectedInvocation.Message.SetValue(exactMessage, MatchType.Exact);
         return this;
      }

      public MessageBoxResponderSetup ExpectMessageExp(string messageRegex) {
         ExpectedInvocation.Message.SetValue(messageRegex, MatchType.Regex);
         return this;
      }

      internal override bool ProcessMessageBoxInvocationCore(DialogServiceInvocation invocation, out CustomDialogResult result) {
         result = DialogResult;

         return
            ExpectedInvocation.Method == invocation.Method &&
            ExpectedInvocation.Caption.Matches(invocation.Caption) &&
            ExpectedInvocation.Message.Matches(invocation.Message);
      }
   }
}
