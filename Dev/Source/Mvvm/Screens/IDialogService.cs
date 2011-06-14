namespace Inspiring.Mvvm.Screens {
   using System;

   /// <summary>
   ///   The icon a message box should have. Abstracts the orginal <see 
   ///   cref="System.Windows.MessageBoxImage"/> enum to be more UI agnostic.
   /// </summary>
   public enum CustomDialogIcon {
      None,
      Error,
      Hand,
      Stop,
      Question,
      Exclamation,
      Warning,
      Information,
      Asterisk
   }

   /// <summary>
   ///   The button the user has clicked on the message box. Abstracts the <see 
   ///   cref="System.Windows.MessageBoxButton"/> enum to be more UI agnostic.
   /// </summary>
   public enum CustomDialogResult {
      None,
      OK,
      Cancel,
      Yes,
      No
   }

   // TODO: Save dialog
   public interface IDialogService {
      void Error(string message, string caption);

      void Info(string message, string caption);

      void Warning(string message, string caption);

      CustomDialogResult YesNo(
         string message,
         string caption,
         CustomDialogResult defaultResult,
         CustomDialogIcon icon = CustomDialogIcon.Question
      );

      CustomDialogResult YesNoCancel(
         string message,
         string caption,
         CustomDialogResult defaultResult,
         CustomDialogIcon icon = CustomDialogIcon.Question
      );

      CustomDialogResult OkCancel(
         string message,
         string caption,
         CustomDialogResult defaultResult,
         CustomDialogIcon icon = CustomDialogIcon.Information
      );

      bool ShowOpenFileDialog(
         IScreenBase parent,
         out string fileName,
         string filter = null,
         string initialDirectory = null
      );

      void Show<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase;

      DialogScreenResult ShowDialog<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase;
   }

   public class DialogScreenResult {
      public DialogScreenResult(bool result, object data = null) {
         Result = result;
         Data = data;
      }

      public bool Result { get; private set; }
      public object Data { get; private set; }
   }

   internal sealed class ExceptionResult {
      public ExceptionResult(Exception exception) {
         Exception = exception;
      }

      public Exception Exception { get; private set; }
   }
}
