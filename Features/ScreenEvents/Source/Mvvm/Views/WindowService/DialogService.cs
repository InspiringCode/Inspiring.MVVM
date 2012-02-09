namespace Inspiring.Mvvm.Views {
   using System;
   using System.Diagnostics.Contracts;
   using System.Windows;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Microsoft.Win32;

   // TODO: Refactor this stuff (see 'WindowService')
   public class DialogService : IDialogService {
      public static readonly Event<EventArgs> DialogOpeningEvent = new Event<EventArgs>();
      public static readonly Event<EventArgs> DialogClosedEvent = new Event<EventArgs>();

      private readonly EventAggregator _aggregator;
      private readonly ScreenService _screenService = new ScreenService();

      public DialogService(IWindowService windowService, EventAggregator aggregator) {
         Contract.Requires<ArgumentNullException>(windowService != null);
         Contract.Requires<ArgumentNullException>(aggregator != null);

         WindowService = windowService;
         _aggregator = aggregator;
      }

      protected IWindowService WindowService { get; private set; }

      public void Show<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase {
         OnDialogOpening();
         ShowCore<TScreen>(screen, parent, title);
         OnDialogClosed();
      }

      public DialogScreenResult ShowDialog<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase {
         OnDialogOpening();
         var result = ShowDialogCore(screen, parent, title);
         OnDialogClosed();

         return result;
      }

      public bool ShowOpenFileDialog(
         IScreenBase parent,
         out string fileName,
         string filter = null,
         string initialDirectory = null
      ) {
         OnDialogOpening();
         var result = ShowOpenFileDialogCore(parent, out fileName, filter, initialDirectory);
         OnDialogClosed();

         return result;
      }

      public bool ShowFolderBrowseDialog(
         IScreenBase parent,
         out string selectedPath,
         string message,
         Environment.SpecialFolder? specialFolder = null
      ) {
         OnDialogOpening();
         var result = ShowFolderBrowseDialogCore(parent, out selectedPath, message, specialFolder);
         OnDialogClosed();

         return result;
      }

      public void Info(string message, string caption) {
         OnDialogOpening();
         InfoCore(message, caption);
         OnDialogClosed();
      }

      public void Warning(string message, string caption) {
         OnDialogOpening();
         WarningCore(message, caption);
         OnDialogClosed();
      }

      public void Error(string message, string caption) {
         OnDialogOpening();
         ErrorCore(message, caption);
         OnDialogClosed();
      }

      public CustomDialogResult YesNo(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Question) {
         OnDialogOpening();
         var result = YesNoCore(message, caption, defaultResult, icon);
         OnDialogClosed();

         return result;
      }

      public CustomDialogResult YesNoCancel(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Question) {
         OnDialogOpening();
         var result = YesNoCancelCore(message, caption, defaultResult, icon);
         OnDialogClosed();

         return result;
      }

      public CustomDialogResult OkCancel(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Information) {
         OnDialogOpening();
         var result = OkCancelCore(message, caption, defaultResult, icon);
         OnDialogClosed();

         return result;
      }

      protected virtual void ShowCore<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase {
         Window window = WindowService.CreateDialogWindow(screen);
         window.Show();
      }

      protected virtual DialogScreenResult ShowDialogCore<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase {
         TScreen s = _screenService.CreateAndActivateScreen(
            screen,
            initializationCallback: x => x.Children.Add(new DialogLifecycle())
         );

         DialogScreenResult result = null;

         try {
            WindowService.ShowDialogWindow(s, parent, title);
         } finally {
            if (DialogLifecycle.HasDialogLifecycle(s)) {
               var dl = DialogLifecycle.GetDialogLifecycle(s);
               s.Children.Remove(dl);

               result = dl.ScreenResult ?? new DialogScreenResult(false);
            }
         }

         if (result.Data is ExceptionResult) {
            throw ((ExceptionResult)result.Data).Exception;
         }

         return result;
      }

      protected virtual bool ShowOpenFileDialogCore(
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

      protected virtual bool ShowFolderBrowseDialogCore(
         IScreenBase parent,
         out string selectedPath,
         string message,
         Environment.SpecialFolder? specialFolder = null
      ) {
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

      protected virtual void InfoCore(string message, string caption) {
         MessageBox.Show(
            message,
            caption,
            MessageBoxButton.OK,
            MessageBoxImage.Information
         );
      }

      protected virtual void WarningCore(string message, string caption) {
         MessageBox.Show(
            message,
            caption,
            MessageBoxButton.OK,
            MessageBoxImage.Warning
         );
      }

      protected virtual void ErrorCore(string message, string caption) {
         MessageBox.Show(
            message,
            caption,
            MessageBoxButton.OK,
            MessageBoxImage.Error
         );
      }

      protected virtual CustomDialogResult YesNoCore(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Question) {
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

      protected virtual CustomDialogResult YesNoCancelCore(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Question) {
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

      protected virtual CustomDialogResult OkCancelCore(string message, string caption, CustomDialogResult defaultResult, CustomDialogIcon icon = CustomDialogIcon.Information) {
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

      protected virtual void OnDialogClosed() {
         _aggregator.Publish(DialogClosedEvent, EventArgs.Empty);
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
