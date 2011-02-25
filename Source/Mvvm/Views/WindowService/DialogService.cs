namespace Inspiring.Mvvm.Views {
   using System;
   using System.Windows;
   using Inspiring.Mvvm.Screens;
   using Microsoft.Win32;

   public class DialogService : IDialogService {
      protected IWindowService WindowService { get; private set; }

      public DialogService(IWindowService windowService) {
         WindowService = windowService;
      }

      public virtual DialogScreenResult ShowDialog<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase {
         IScreenBase s = screen.Create();
         s.Children.Add(new DialogLifecycle());

         WindowService.ShowDialogWindow(s, parent, title);

         var dl = DialogLifecycle.GetDialogLifecycle(s);
         return dl.ScreenResult ?? new DialogScreenResult(false);
      }

      public virtual bool ShowOpenFileDialog(
         IScreenBase parent,
         out string fileName,
         string filter = null,
         string initialDirectory = null
      ) {
         Window owner = WindowService.GetAssociatedWindow(parent);
         OpenFileDialog ofd = new OpenFileDialog();

         if (filter != null) {
            ofd.Filter = filter;
         }

         if (initialDirectory != null) {
            ofd.InitialDirectory = initialDirectory;
         }

         var result = ofd.ShowDialog(owner);
         fileName = ofd.FileName;
         return result.Value;
      }

      public virtual void Info(string message, string caption) {
         MessageBox.Show(
            message,
            caption,
            MessageBoxButton.OK,
            MessageBoxImage.Information
         );
      }

      public virtual void Warning(string message, string caption) {
         MessageBox.Show(
            message,
            caption,
            MessageBoxButton.OK,
            MessageBoxImage.Warning
         );
      }

      public virtual void Error(string message, string caption) {
         MessageBox.Show(
            message,
            caption,
            MessageBoxButton.OK,
            MessageBoxImage.Error
         );
      }

      public virtual CustomDialogResult YesNo(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Question) {
         return MapResult(
            MessageBox.Show(
               message,
               caption,
               MessageBoxButton.YesNo,
               MapIcon(icon),
               MapResult(defaultResult)
            )
         );
      }

      public virtual CustomDialogResult YesNoCancel(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Question) {
         return MapResult(
            MessageBox.Show(
               message,
               caption,
               MessageBoxButton.YesNoCancel,
               MapIcon(icon),
               MapResult(defaultResult)
            )
         );
      }

      public virtual CustomDialogResult OkCancel(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Information) {
         return MapResult(
            MessageBox.Show(
               message,
               caption,
               MessageBoxButton.OKCancel,
               MapIcon(icon),
               MapResult(defaultResult)
            )
         );
      }

      protected static MessageBoxImage MapIcon(CustomDialogIcon icon) {
         switch (icon) {
            case CustomDialogIcon.Exclamation:
               return MessageBoxImage.Exclamation;
            case CustomDialogIcon.Information:
               return MessageBoxImage.Information;
            case CustomDialogIcon.None:
               return MessageBoxImage.None;
            case CustomDialogIcon.Question:
               return MessageBoxImage.Question;
            case CustomDialogIcon.Stop:
               return MessageBoxImage.Stop;
            case CustomDialogIcon.Warning:
               return MessageBoxImage.Warning;
            default:
               throw new ArgumentException();
         }
      }

      protected static MessageBoxResult MapResult(CustomDialogResult result) {
         switch (result) {
            case CustomDialogResult.Cancel:
               return MessageBoxResult.Cancel;
            case CustomDialogResult.No:
               return MessageBoxResult.No;
            case CustomDialogResult.None:
               return MessageBoxResult.None;
            case CustomDialogResult.OK:
               return MessageBoxResult.OK;
            case CustomDialogResult.Yes:
               return MessageBoxResult.Yes;
            default:
               throw new ArgumentException();
         }
      }

      protected static CustomDialogResult MapResult(MessageBoxResult result) {
         switch (result) {
            case MessageBoxResult.Cancel:
               return CustomDialogResult.Cancel;
            case MessageBoxResult.No:
               return CustomDialogResult.No;
            case MessageBoxResult.None:
               return CustomDialogResult.None;
            case MessageBoxResult.OK:
               return CustomDialogResult.OK;
            case MessageBoxResult.Yes:
               return CustomDialogResult.Yes;
            default:
               throw new ArgumentException();
         }
      }
   }
}
