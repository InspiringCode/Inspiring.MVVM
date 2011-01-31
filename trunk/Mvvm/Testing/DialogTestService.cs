namespace Inspiring.Mvvm.Testing {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Screens;

   public class DialogTestService<TScreen> : IDialogService where TScreen : IScreenBase {
      private Stack<Func<TScreen, DialogScreenResult>> _actions =
         new Stack<Func<TScreen, DialogScreenResult>>();

      public void AddTestAction(Func<TScreen, DialogScreenResult> action) {
         Contract.Requires<ArgumentNullException>(action != null);
         _actions.Push(action);
      }

      public bool ShowOpenFileDialog(
         IScreenBase parent,
         out string fileName,
         string filter = null,
         string initialDirectory = null
      ) {
         throw new NotImplementedException();
      }

      public DialogScreenResult ShowDialog<T>(
         IScreenFactory<T> screen,
         IScreenBase parent,
         string title = null
      ) where T : IScreenBase {
         IScreenBase s = screen.Create(x => { });
         s.Children.Add(new DialogLifecycle());

         var action = _actions.Pop();
         action((TScreen)s);

         var dl = DialogLifecycle.GetDialogLifecycle(s);
         return dl.ScreenResult ?? new DialogScreenResult(false);
      }

      public void Error(string message, string caption) {
         throw new NotImplementedException();
      }

      public void Info(string message, string caption) {
         throw new NotImplementedException();
      }

      public void Warning(string message, string caption) {
         throw new NotImplementedException();
      }

      public CustomDialogResult YesNo(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Question) {
         throw new NotImplementedException();
      }

      public CustomDialogResult YesNoCancel(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Question) {
         throw new NotImplementedException();
      }

      public CustomDialogResult OkCancel(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Information) {
         throw new NotImplementedException();
      }
   }
}
