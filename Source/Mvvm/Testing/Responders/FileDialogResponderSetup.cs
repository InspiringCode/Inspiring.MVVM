namespace Inspiring.Mvvm.Testing {
   using Inspiring.Mvvm.Screens;

   public sealed class FileDialogResponderSetup : ResponderBase {
      private readonly string _fileName;

      internal FileDialogResponderSetup(
         DialogServiceMethod method,
         string fileName,
         bool result
      )
         : base(method) {
         _fileName = fileName;
         DialogResult = result;
      }

      internal bool DialogResult {
         get;
         private set;
      }

      public FileDialogResponderSetup ExpectInitialDirectory(string exactInitialDirectory) {
         ExpectedInvocation.InitialDirectory.SetValue(exactInitialDirectory, MatchType.Exact);
         return this;
      }

      public FileDialogResponderSetup ExpectInitialDirectoryExp(string initialDirectoryRegex) {
         ExpectedInvocation.InitialDirectory.SetValue(initialDirectoryRegex, MatchType.Regex);
         return this;
      }

      public FileDialogResponderSetup ExpectFilter(string filterMessage) {
         ExpectedInvocation.Filter.SetValue(filterMessage, MatchType.Exact);
         return this;
      }

      public FileDialogResponderSetup ExpectFilterExp(string filterRegex) {
         ExpectedInvocation.Filter.SetValue(filterRegex, MatchType.Regex);
         return this;
      }

      public FileDialogResponderSetup ExpectParent(IScreenBase parent) {
         ExpectedInvocation.Parent.SetValue(parent);
         return this;
      }

      internal override bool ProcessFileDialogInvocationCore(
         DialogServiceInvocation invocation,
         out string fileName,
         out bool result
      ) {
         fileName = _fileName;
         result = DialogResult;

         return
            ExpectedInvocation.Method == invocation.Method &&
            ExpectedInvocation.InitialDirectory.Matches(invocation.InitialDirectory) &&
            ExpectedInvocation.Filter.Matches(invocation.Filter);
      }
   }
}
