namespace Inspiring.Mvvm.Testing {
   using System;
   using System.Windows;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Views;
using Inspiring.Mvvm.Common;

   public abstract class ShowDialogResponderSetup : ResponderBase {
      internal ShowDialogResponderSetup(DialogServiceMethod method)
         : base(method) {
      }

      public ShowDialogResponderSetup ExpectCaption(string exactCaption) {
         ExpectedInvocation.Caption.SetValue(exactCaption, MatchType.Exact);
         return this;
      }

      public ShowDialogResponderSetup ExpectCaptionExp(string captionRegex) {
         ExpectedInvocation.Caption.SetValue(captionRegex, MatchType.Regex);
         return this;
      }

      public ShowDialogResponderSetup ExpectParent(IScreenBase parent) {
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
            result = ProcessInvocation(invocation, screen);
         }

         return match;
      }

      internal abstract DialogScreenResult ProcessInvocation<T>(
         DialogServiceInvocation invocation,
         IScreenFactory<T> screen
      ) where T : IScreenBase;
   }

   public sealed class ShowDialogResponderSetup<TScreen> :
      ShowDialogResponderSetup
      where TScreen : IScreenBase {

      private EventAggregator _aggregator;

      internal ShowDialogResponderSetup(DialogServiceMethod method, Action<TScreen> dialogTestAction, EventAggregator aggregator)
         : base(method) {
         DialogTestAction = dialogTestAction;
         _aggregator = aggregator;
      }

      internal Action<TScreen> DialogTestAction { get; private set; }

      internal override DialogScreenResult ProcessInvocation<T>(
         DialogServiceInvocation invocation,
         IScreenFactory<T> screen
      ) {
         IScreenBase s = screen.Create(_aggregator);
         s.Children.Add(new DialogLifecycle(s));
         
         IScreenBase parent = (IScreenBase)invocation.Parent.Value;

         DialogTestAction((TScreen)s);

         var dl = DialogLifecycle.GetDialogLifecycle(s);
         return dl.ScreenResult ?? new DialogScreenResult(false);
      }
   }

   public sealed class ShowDialogResponderResultSetup : ShowDialogResponderSetup {
      private readonly DialogScreenResult _dialogResult;

      internal ShowDialogResponderResultSetup(DialogServiceMethod method, DialogScreenResult dialogResult)
         : base(method) {
         _dialogResult = dialogResult;
      }

      internal override DialogScreenResult ProcessInvocation<T>(
         DialogServiceInvocation invocation,
         IScreenFactory<T> screen
      ) {
         return _dialogResult;
      }
   }
}
