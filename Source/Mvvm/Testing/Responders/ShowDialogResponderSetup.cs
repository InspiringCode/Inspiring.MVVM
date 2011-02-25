namespace Inspiring.Mvvm.Testing {
   using System;
   using Inspiring.Mvvm.Screens;

   public sealed class ShowDialogResponderSetup<TScreen> :
      ResponderBase
      where TScreen : IScreenBase {

      internal ShowDialogResponderSetup(DialogServiceMethod method, Action<TScreen> dialogTestAction)
         : base(method) {
         DialogTestAction = dialogTestAction;
      }

      public Action<TScreen> DialogTestAction { get; private set; }

      public ShowDialogResponderSetup<TScreen> ExpectCaption(string exactCaption) {
         ExpectedInvocation.Caption.SetValue(exactCaption, MatchType.Exact);
         return this;
      }

      public ShowDialogResponderSetup<TScreen> ExpectCaptionExp(string captionRegex) {
         ExpectedInvocation.Caption.SetValue(captionRegex, MatchType.Regex);
         return this;
      }

      public ShowDialogResponderSetup<TScreen> ExpectParent(IScreenBase parent) {
         ExpectedInvocation.Parent.SetValue(parent);
         return this;
      }

      internal override bool ProcessScreenDialogInvocationCore<T>(
         DialogServiceInvocation invocation,
         IScreenFactory<T> screen,
         out DialogScreenResult result
      ) {
         result = null;

         bool match =
            ExpectedInvocation.Method == invocation.Method &&
            ExpectedInvocation.Caption.Matches(invocation.Caption) &&
            ExpectedInvocation.Parent.Matches(invocation.Parent);

         if (match) {
            IScreenBase s = screen.Create();
            s.Children.Add(new DialogLifecycle());

            DialogTestAction((TScreen)s);

            var dl = DialogLifecycle.GetDialogLifecycle(s);
            result = dl.ScreenResult ?? new DialogScreenResult(false);
         }

         return match;
      }
   }
}
