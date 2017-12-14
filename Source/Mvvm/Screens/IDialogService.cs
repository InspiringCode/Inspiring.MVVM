namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;

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
         string initialDirectory = null,
         IEnumerable<string> customPlaces = null
      );

      bool ShowSaveFileDialog(
         IScreenBase parent,
         ref string fileName,
         string filter = null,
         string initialDirectory = null,
         IEnumerable<string> customPlaces = null
      );

      bool ShowFolderBrowseDialog(
         IScreenBase parent,
         out string selectedPath,
         string message,
         Nullable<Environment.SpecialFolder> specialFolder = null
      );

      void Show(
         IScreenFactory<IScreenBase> screen,
         IScreenBase parent = null,
         string title = null
      );

      DialogScreenResult ShowDialog(
         IScreenFactory<IScreenBase> screen,
         IScreenBase parent = null,
         string title = null
      );
   }

   public class DialogScreenResult {
      public DialogScreenResult(bool result, object data = null) {
         Result = result;
         Data = data;
      }

      public bool Result { get; private set; }

      // TODO: Document these properties and maybe add extension methods for setting appropriate results.

      public bool IsOK {
         get { return Result; }
      }

      public bool IsCancelled {
         get { return !Result; }
      }

      public object Data { get; private set; }
   }
}
