﻿namespace Inspiring.Mvvm.Views {
   using System;
   using System.ComponentModel;
   using System.Windows;
   using System.Windows.Media;
   using Inspiring.Mvvm.Screens;
   using Microsoft.Win32;

   public class WindowService : IWindowService, IDialogService {
      public virtual Window CreateWindow<TScreen>(
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreenBase {
         Window window = CreateWindow();
         ConfigureWindow(window, forScreen);
         return window;
      }
      public virtual Window CreateDialogWindow<TScreen>(
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreenBase {
         Window window = CreateDialogWindow();
         ConfigureDialogWindow(window, forScreen);
         return window;
      }
      public virtual Window CreateShellWindow<TScreen>(
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreenBase {
         Window window = CreateShellWindow();
         ConfigureWindow(window, forScreen);
         return window;
      }

      public void ConfigureWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreenBase {
         IScreenBase s = forScreen.Create(x => { });
         ConfigureWindow(window, s, new WindowCloseHandler(s));
      }

      public void ConfigureDialogWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> forScreen
      ) where TScreen : IScreenBase {
         IScreenBase s = forScreen.Create(x => { });
         s.Children.Add(new DialogLifecycle());
         ConfigureWindow(window, s, new DialogCloseHandler(s));
      }

      protected virtual void ConfigureWindow(
         Window window,
         IScreenBase forScreen,
         WindowCloseHandler closeHandler
      ) {
         // Save the window for later
         forScreen.Children.Add(new WindowLifecycle {
            AssociatedWindow = window
         });

         IScreenBase screen = forScreen;
         screen.Activate();

         // TryInitialize succeeds if the window implements 'IView<TScreen>'.
         if (!ViewFactory.TryInitializeView(window, screen)) {
            // Resolve a new view for 'TScreen'.
            window.Content = ViewFactory.CreateView(screen);
         }

         closeHandler.AttachTo(window);
      }

      // TODO: Refactor the whole WindowService/DialogService stuff...
      public DialogScreenResult ShowDialog<TScreen>(
         Window dialogWindow,
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null
      ) where TScreen : IScreenBase {
         IScreenBase s = screen.Create(x => { });
         s.Children.Add(new DialogLifecycle());
         ConfigureWindow(dialogWindow, s, new DialogCloseHandler(s));

         if (parent != null) {
            Window owner = GetAssociatedWindow(parent);
            dialogWindow.Owner = owner;
         }

         dialogWindow.ShowInTaskbar = false;
         dialogWindow.ShowDialog();

         var dl = DialogLifecycle.GetDialogLifecycle(s);
         return dl.ScreenResult ?? new DialogScreenResult(false);
      }

      public DialogScreenResult ShowDialog<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase {


         Window dialogWindow = CreateDialogWindow();

         if (title != null) {
            dialogWindow.Title = title;
         }

         return ShowDialog(dialogWindow, screen, parent);
      }

      public bool ShowOpenFileDialog(
         IScreenBase parent,
         out string fileName,
         string filter = null,
         string initialDirectory = null
      ) {
         Window owner = GetAssociatedWindow(parent);
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

      protected virtual Window CreateWindow() {
         Window window = new Window();

         // Needed for sharp text rendering.
         TextOptions.SetTextFormattingMode(window, TextFormattingMode.Display);
         TextOptions.SetTextRenderingMode(window, TextRenderingMode.Aliased);

         return window;
      }

      protected virtual Window CreateDialogWindow() {
         return CreateWindow();
      }

      protected virtual Window CreateShellWindow() {
         return CreateWindow();
      }

      protected void AttachWindowCloseHandlers(Window window, IScreenBase screen) {
         window.Closing += delegate(object sender, CancelEventArgs e) {
            e.Cancel = !screen.RequestClose();
         };

         window.Closed += delegate(object sender, EventArgs e) {
            screen.Deactivate();
            screen.Close();
         };
      }

      protected void AttachDialogCloseHandlers(Window window, ScreenBase screen) {
         var dl = DialogLifecycle.GetDialogLifecycle(screen);

         bool userRequestedClose = true;

         dl.CloseWindow += delegate {
            window.Close();
            userRequestedClose = false;
         };


         IScreenBase s = screen;

         window.Closing += delegate(object sender, CancelEventArgs e) {
            dl.WindowResult = ((Window)sender).DialogResult;
            if (userRequestedClose) {

            }
         };

         window.Closed += delegate(object sender, EventArgs e) {
            dl.WindowResult = ((Window)sender).DialogResult;
            s.Deactivate();
            s.Close();
         };
      }

      protected Window GetAssociatedWindow(IScreenBase ofScreen) {
         // TODO: Can be generalize this logic (traversing of hierarchy)?
         for (IScreenLifecycle s = ofScreen; s != null; s = s.Parent) {
            IScreenBase p = s as IScreenBase;
            if (p != null && p.Children.Contains<WindowLifecycle>()) {
               return p
                  .Children
                  .Expose<WindowLifecycle>()
                  .AssociatedWindow;
            }
         }

         throw new ArgumentException(ExceptionTexts.NoAssociatedWindow);
      }

      protected class DialogCloseHandler : WindowCloseHandler {
         private IScreenBase _dialog;
         private bool _closeIsUserRequested = true;

         public DialogCloseHandler(IScreenBase dialog)
            : base(dialog) {
            _dialog = dialog;
         }

         public override void AttachTo(Window window) {
            base.AttachTo(window);
            DialogLifecycle
               .GetDialogLifecycle(_dialog)
               .CloseWindow += (sender, e) => {
                  _closeIsUserRequested = false;
                  window.Close();
               };
         }

         protected override bool OnClosing(Window window) {
            DialogLifecycle
               .GetDialogLifecycle(_dialog)
               .WindowResult = window.DialogResult;

            return _closeIsUserRequested ?
               base.OnClosing(window) :
               true;
         }

         protected override void OnClosed(Window window) {
            DialogLifecycle
               .GetDialogLifecycle(_dialog)
               .WindowResult = window.DialogResult;

            base.OnClosed(window);
         }
      }

      protected class WindowCloseHandler {
         private IScreenBase _screen;

         public WindowCloseHandler(IScreenBase screen) {
            _screen = screen;
         }
         public virtual void AttachTo(Window window) {
            window.Closing += (sender, e) => {
               e.Cancel = !OnClosing((Window)sender);
            };

            window.Closed += (sender, e) => {
               OnClosed((Window)sender);
            };
         }

         protected virtual bool OnClosing(Window window) {
            return _screen.RequestClose();
         }

         protected virtual void OnClosed(Window window) {
            _screen.Deactivate();
            _screen.Close();
         }
      }

      private class WindowLifecycle : ScreenLifecycle {
         public Window AssociatedWindow { get; set; }
      }
   }
}
