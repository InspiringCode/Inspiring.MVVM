namespace Inspiring.Mvvm.Testing {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;

   public class DialogTestService : IDialogService {
      private Queue<ResponderBase> _responders = new Queue<ResponderBase>();
      private EventAggregator _aggregator;

      public DialogTestService(EventAggregator aggregator) {
         _aggregator = aggregator;
      }

      public ShowDialogResponderSetup<TScreen> EnqueueShowDialogResponder<TScreen>(
         Action<TScreen> dialogTestAction
      ) where TScreen : IScreenBase {
         var resp = new ShowDialogResponderSetup<TScreen>(DialogServiceMethod.OpenDialog, dialogTestAction, _aggregator);
         _responders.Enqueue(resp);
         return resp;
      }

      public ShowDialogResponderResultSetup EnqueueShowDialogResponder(DialogScreenResult result) {
         var resp = new ShowDialogResponderResultSetup(DialogServiceMethod.OpenDialog, result);
         _responders.Enqueue(resp);
         return resp;
      }

      public FileDialogResponderSetup EnqueueOpenFileDialogResponder(string fileName, bool result) {
         var resp = new FileDialogResponderSetup(DialogServiceMethod.ShowOpenFileDialog, fileName, result);
         _responders.Enqueue(resp);
         return resp;
      }

      public FolderDialogResponderSetup EnqueueFolderBrowserDialogResponder(string selectedPath, bool result) {
         var resp = new FolderDialogResponderSetup(selectedPath, result);
         _responders.Enqueue(resp);
         return resp;
      }

      public MessageBoxResponderSetup EnqueueInfoResponder() {
         return EnqueueMessageBoxResponder(DialogServiceMethod.Info);
      }

      public MessageBoxResponderSetup EnqueueWarningResponder() {
         return EnqueueMessageBoxResponder(DialogServiceMethod.Warning);
      }

      public MessageBoxResponderSetup EnqueueErrorResponder() {
         return EnqueueMessageBoxResponder(DialogServiceMethod.Error);
      }

      public MessageBoxResponderSetup EnqueueYesNoResponder(CustomDialogResult result) {
         return EnqueueMessageBoxResponder(DialogServiceMethod.YesNo, result);
      }

      public MessageBoxResponderSetup EnqueueYesNoCancelResponder(CustomDialogResult result) {
         return EnqueueMessageBoxResponder(DialogServiceMethod.YesNoCancel, result);
      }

      public MessageBoxResponderSetup EnqueueOkCancelResponder(CustomDialogResult result) {
         return EnqueueMessageBoxResponder(DialogServiceMethod.OkCancel, result);
      }

      public void VerifyAllRespondersWereCalled() {
         if (_responders.Count > 0) {
            throw new ArgumentException();
         }
      }

      public TestScreenResult ShowDialog<TScreen>(
        IScreenFactory<TScreen> screen
      ) where TScreen : IScreenBase {
         IScreenBase s = screen.Create(_aggregator);
         var lifecycle = new DialogLifecycle(s);
         s.Children.Add(lifecycle);
         s.Children.Add(new ScreenCloseHandler(_ => { })); // TODO: Remove code duplication between here and responder
         return new TestScreenResult(lifecycle);
      }

      DialogScreenResult IDialogService.ShowDialog(IScreenFactory<IScreenBase> screen, IScreenBase parent, string title) {
         var invocation = new DialogServiceInvocation(DialogServiceMethod.OpenDialog);
         invocation.Caption.SetValue(title);
         invocation.Parent.SetValue(parent);
         return DequeueResponder().ProcessScreenDialogInvocation(invocation, screen);
      }

      void IDialogService.Show(IScreenFactory<IScreenBase> screen, IScreenBase parent, string title) {
         // TODO
         throw new NotImplementedException();
      }

      bool IDialogService.ShowOpenFileDialog(IScreenBase parent, out string fileName, string filter, string initialDirectory) {
         var invocation = new DialogServiceInvocation(DialogServiceMethod.ShowOpenFileDialog);
         invocation.Filter.SetValue(filter);
         invocation.InitialDirectory.SetValue(initialDirectory);
         invocation.Parent.SetValue(parent);

         return DequeueResponder().ProcessFileDialogInvocation(invocation, out fileName);
      }

      bool IDialogService.ShowFolderBrowseDialog(IScreenBase parent, out string selectedPath, string message, Environment.SpecialFolder? specialFolder = null) {
         var invocation = new DialogServiceInvocation(DialogServiceMethod.ShowOpenFileDialog);
         invocation.Message.SetValue(message);
         invocation.Parent.SetValue(parent);

         return DequeueResponder().ProcessFolderDialogInvocation(invocation, out selectedPath);
      }

      void IDialogService.Error(string message, string caption) {
         ProcessMessageBoxInvocation(DialogServiceMethod.Error, message, caption);
      }

      void IDialogService.Info(string message, string caption) {
         ProcessMessageBoxInvocation(DialogServiceMethod.Info, message, caption);
      }

      void IDialogService.Warning(string message, string caption) {
         ProcessMessageBoxInvocation(DialogServiceMethod.Warning, message, caption);
      }

      CustomDialogResult IDialogService.YesNo(
         string message,
         string caption,
         CustomDialogResult defaultResult,
         CustomDialogIcon icon
      ) {
         return ProcessMessageBoxInvocation(DialogServiceMethod.YesNo, message, caption);
      }

      CustomDialogResult IDialogService.YesNoCancel(
         string message,
         string caption,
         CustomDialogResult defaultResult,
         CustomDialogIcon icon
      ) {
         return ProcessMessageBoxInvocation(DialogServiceMethod.YesNoCancel, message, caption);
      }

      CustomDialogResult IDialogService.OkCancel(
         string message,
         string caption,
         CustomDialogResult defaultResult,
         CustomDialogIcon icon
      ) {
         return ProcessMessageBoxInvocation(DialogServiceMethod.OkCancel, message, caption);
      }

      private MessageBoxResponderSetup EnqueueMessageBoxResponder(
         DialogServiceMethod type,
         CustomDialogResult result = CustomDialogResult.None
      ) {
         MessageBoxResponderSetup responder = new MessageBoxResponderSetup(type, result);
         _responders.Enqueue(responder);
         return responder;
      }

      private CustomDialogResult ProcessMessageBoxInvocation(DialogServiceMethod type, string message, string caption) {
         var invocation = new DialogServiceInvocation(type);
         invocation.Message.SetValue(message);
         invocation.Caption.SetValue(caption);

         return DequeueResponder().ProcessMessageBoxInvocation(invocation);
      }

      private ResponderBase DequeueResponder() {
         if (_responders.Count == 0) {
            throw new ArgumentException();
         }

         return _responders.Dequeue();
      }
   }
}
