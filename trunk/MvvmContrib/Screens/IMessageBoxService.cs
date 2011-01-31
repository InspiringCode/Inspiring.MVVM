//using System;
//namespace Inspiring.Mvvm.Screens {
//   /// <summary>
//   ///   The icon a message box should have. Abstracts the orginal <see 
//   ///   cref="System.Windows.MessageBoxImage"/> enum to be more UI agnostic.
//   /// </summary>
//   public enum CustomDialogIcon {
//      None,
//      Information,
//      Question,
//      Exclamation,
//      Stop,
//      Warning
//   }

//   /// <summary>
//   ///   The button the user has clicked on the message box. Abstracts the <see 
//   ///   cref="System.Windows.MessageBoxButton"/> enum to be more UI agnostic.
//   /// </summary>
//   public enum CustomDialogResult {
//      None,
//      OK,
//      Cancel,
//      Yes,
//      No
//   }

//   // TODO: Provide default WPF implementation
//   [Obsolete]
//   public interface IMessageBoxService {
//      void Error(string message, string caption);

//      void Info(string message, string caption);

//      void Warning(string message, string caption);

//      CustomDialogResult YesNo(
//         string message,
//         string caption,
//         CustomDialogResult defaultResult,
//         CustomDialogIcon icon = CustomDialogIcon.Question
//      );

//      CustomDialogResult YesNoCancel(
//         string message,
//         string caption,
//         CustomDialogResult defaultResult,
//         CustomDialogIcon icon = CustomDialogIcon.Question
//      );

//      CustomDialogResult OkCancel(
//         string message,
//         string caption,
//         CustomDialogResult defaultResult,
//         CustomDialogIcon icon = CustomDialogIcon.Information
//      );
//   }
//}
