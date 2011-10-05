namespace Inspiring.Mvvm.Views {
   using System;
   using System.Diagnostics.Contracts;
   using System.Windows;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Microsoft.Win32;

   public class DialogService : IDialogService {
      public static readonly Event<EventArgs> DialogOpeningEvent = new Event<EventArgs>();

      private readonly EventAggregator _aggregator;

      public DialogService(IWindowService windowService, EventAggregator aggregator) {
         Contract.Requires<ArgumentNullException>(windowService != null);
         Contract.Requires<ArgumentNullException>(aggregator != null);

         WindowService = windowService;
         _aggregator = aggregator;
      }

      protected IWindowService WindowService { get; private set; }

      public virtual void Show<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase {
         OnDialogOpening();

         Window window = WindowService.CreateDialogWindow(screen);
         window.Show();
      }

      public virtual DialogScreenResult ShowDialog<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase {
         OnDialogOpening();

         IScreenBase s = screen.Create();
         s.Children.Add(new DialogLifecycle());

         WindowService.ShowDialogWindow(s, parent, title);

         var dl = DialogLifecycle.GetDialogLifecycle(s);
         s.Children.Remove(dl);
         var result = dl.ScreenResult ?? new DialogScreenResult(false);

         if (result.Data is ExceptionResult) {
            throw ((ExceptionResult)result.Data).Exception;
         }

         return result;
      }

      public virtual bool ShowOpenFileDialog(
         IScreenBase parent,
         out string fileName,
         string filter = null,
         string initialDirectory = null
      ) {
         OnDialogOpening();

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

      public bool ShowFolderBrowseDialog(
         IScreenBase parent,
         out string selectedPath,
         string message,
         Environment.SpecialFolder? specialFolder = null
      ) {
         OnDialogOpening();

         Window owner = WindowService.GetAssociatedWindow(parent);

         FolderBrowserDialog fbd = new FolderBrowserDialog();
         if (specialFolder != null) {
            fbd.RootFolder = (Environment.SpecialFolder)specialFolder;
         }

         fbd.Description = message;

         bool result = fbd.ShowDialog(owner);

         selectedPath = result ? fbd.SelectedPath : String.Empty;

         return result;
      }

      public virtual void Info(string message, string caption) {
         OnDialogOpening();

         MessageBox.Show(
            message,
            caption,
            MessageBoxButton.OK,
            MessageBoxImage.Information
         );
      }

      public virtual void Warning(string message, string caption) {
         OnDialogOpening();

         MessageBox.Show(
            message,
            caption,
            MessageBoxButton.OK,
            MessageBoxImage.Warning
         );
      }

      public virtual void Error(string message, string caption) {
         OnDialogOpening();

         MessageBox.Show(
            message,
            caption,
            MessageBoxButton.OK,
            MessageBoxImage.Error
         );
      }

      public virtual CustomDialogResult YesNo(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Question) {
         OnDialogOpening();

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
         OnDialogOpening();

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
         OnDialogOpening();

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

      protected virtual void OnDialogOpening() {
         _aggregator.Publish(DialogOpeningEvent, EventArgs.Empty);
      }

      protected static MessageBoxImage MapIcon(CustomDialogIcon icon) {
         switch (icon) {
            case CustomDialogIcon.None:
               return MessageBoxImage.None;
            case CustomDialogIcon.Error:
               return MessageBoxImage.Error;
            case CustomDialogIcon.Hand:
               return MessageBoxImage.Hand;
            case CustomDialogIcon.Stop:
               return MessageBoxImage.Stop;
            case CustomDialogIcon.Question:
               return MessageBoxImage.Question;
            case CustomDialogIcon.Exclamation:
               return MessageBoxImage.Exclamation;
            case CustomDialogIcon.Warning:
               return MessageBoxImage.Warning;
            case CustomDialogIcon.Information:
               return MessageBoxImage.Information;
            case CustomDialogIcon.Asterisk:
               return MessageBoxImage.Asterisk;
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
