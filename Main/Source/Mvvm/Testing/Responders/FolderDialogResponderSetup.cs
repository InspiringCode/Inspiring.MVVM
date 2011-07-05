using Inspiring.Mvvm.Screens;
namespace Inspiring.Mvvm.Testing {

   public sealed class FolderDialogResponderSetup : ResponderBase {
      private readonly string _selectedPath;

      internal FolderDialogResponderSetup(
         string selectedPath,
         bool result
      )
         : base(DialogServiceMethod.ShowFolderBrowseDialog) {
         _selectedPath = selectedPath;
         DialogResult = result;
      }

      internal bool DialogResult {
         get;
         private set;
      }

      public FolderDialogResponderSetup ExpectMessage(string exactMessage) {
         ExpectedInvocation.Message.SetValue(exactMessage, MatchType.Exact);
         return this;
      }

      public FolderDialogResponderSetup ExpectMessageExp(string messageRegex) {
         ExpectedInvocation.Message.SetValue(messageRegex, MatchType.Regex);
         return this;
      }

      public FolderDialogResponderSetup ExpectParent(IScreenBase parent) {
         ExpectedInvocation.Parent.SetValue(parent);
         return this;
      }

      internal override bool ProcessFolderDialogInvocationCore(
         DialogServiceInvocation invocation,
         out string selectedPath,
         out bool result
      ) {
         selectedPath = _selectedPath;
         result = DialogResult;

         return
            ExpectedInvocation.Method == invocation.Method &&
            ExpectedInvocation.Message.Matches(invocation.Message);
      }
   }
}
