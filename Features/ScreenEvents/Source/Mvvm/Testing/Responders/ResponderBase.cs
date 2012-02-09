namespace Inspiring.Mvvm.Testing {
   using System;
   using Inspiring.Mvvm.Screens;

   public abstract class ResponderBase {
      internal ResponderBase(DialogServiceMethod method) {
         ExpectedInvocation = new DialogServiceInvocation(method);
      }

      internal DialogServiceInvocation ExpectedInvocation {
         get;
         private set;
      }

      internal CustomDialogResult ProcessMessageBoxInvocation(
         DialogServiceInvocation invocation
      ) {
         CustomDialogResult dialogResult;

         if (!ProcessMessageBoxInvocationCore(invocation, out dialogResult)) {
            ThrowAssertionException(invocation);
         }

         return dialogResult;
      }

      internal DialogScreenResult ProcessScreenDialogInvocation<TScreen>(
         DialogServiceInvocation invocation,
         IScreenFactory<TScreen> screen
      ) where TScreen : IScreenBase {
         DialogScreenResult dialogResult;

         if (!ProcessScreenDialogInvocationCore(invocation, screen, out dialogResult)) {
            ThrowAssertionException(invocation);
         }

         return dialogResult;
      }

      internal bool ProcessFileDialogInvocation(
         DialogServiceInvocation invocation,
         out string filename
      ) {
         bool dialogResult;

         if (!ProcessFileDialogInvocationCore(invocation, out filename, out dialogResult)) {
            ThrowAssertionException(invocation);
         }

         return dialogResult;
      }

      internal bool ProcessFolderDialogInvocation(
         DialogServiceInvocation invocation,
         out string selectedPath
      ) {
         bool dialogResult;

         if (!ProcessFolderDialogInvocationCore(invocation, out selectedPath, out dialogResult)) {
            ThrowAssertionException(invocation);
         }

         return dialogResult;
      }

      internal virtual bool ProcessMessageBoxInvocationCore(
         DialogServiceInvocation invocation,
         out CustomDialogResult result
      ) {
         result = CustomDialogResult.None;
         return false;
      }

      internal virtual bool ProcessFileDialogInvocationCore(
         DialogServiceInvocation invocation,
         out string filename,
         out bool result
      ) {
         filename = null;
         result = false;
         return false;
      }


      internal virtual bool ProcessFolderDialogInvocationCore(
         DialogServiceInvocation invocation,
         out string selectedPath,
         out bool result
      ) {
         selectedPath = null;
         result = false;
         return false;
      }

      internal virtual bool ProcessScreenDialogInvocationCore<TScreen>(
         DialogServiceInvocation invocation,
         IScreenFactory<TScreen> screen,
         out DialogScreenResult result
      ) where TScreen : IScreenBase {
         result = null;
         return false;
      }

      private void ThrowAssertionException(
         DialogServiceInvocation actualInvocation
      ) {
         throw new ArgumentException();
      }
   }
}
