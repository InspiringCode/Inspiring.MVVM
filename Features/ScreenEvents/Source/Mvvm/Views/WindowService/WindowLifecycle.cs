namespace Inspiring.Mvvm.Views {
   using System;
   using System.ComponentModel;
   using System.Windows;
   using System.Windows.Threading;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;

   internal sealed class WindowLifecycle {
      private readonly EventAggregator _aggregator;
      private readonly IWindowService _windowService;
      private readonly Window _window;
      private readonly ScreenCloseHandler _closeHandler;

      private IScreenBase _screen;
      private ScreenLifecycleOperations _screenOps;
      private ScreenLifecycleException _lastWindowHandlerException;
      private bool _modal;

      public WindowLifecycle(
         EventAggregator aggregator,
         IWindowService windowService,
         Window window
      ) {
         _aggregator = aggregator;
         _windowService = windowService;
         _window = window;
         _closeHandler = new ScreenCloseHandler(HandleClose);
      }

      public Window Window {
         get { return _window; }
      }

      public static DialogScreenResult Show(
         EventAggregator aggregator,
         IWindowService windowService,
         Window window,
         IScreenFactory<IScreenBase> screenFactory,
         bool modal
      ) {
         var lf = new WindowLifecycle(aggregator, windowService, window);

         if (modal) {
            return lf.ShowModal(screenFactory);
         } else {
            lf.Show(screenFactory);
            return null;
         }
      }

      private void Show(IScreenFactory<IScreenBase> screenFactory) {
         _modal = false;

         try {
            _screen = screenFactory.Create(_aggregator);
            Initialize();
            _windowService.ShowWindow(_window, modal: false);
         } finally {
            Disconnect();
         }
      }

      private DialogScreenResult ShowModal(IScreenFactory<IScreenBase> screenFactory) {
         _modal = true;

         DialogLifecycle dialogLifecycle = null;

         try {
            _screen = screenFactory.Create(
               _aggregator,
               s => {
                  dialogLifecycle = new DialogLifecycle(s);
                  s.Children.Add(dialogLifecycle);
               }
            );

            Initialize();

            try {
               _windowService.ShowWindow(_window, modal: true);
            } catch (Exception ex) {
               // If an exception is thrown in the UI code of the dialog window (e.g. in
               // a command handler or on a Dispatcher action), ShowDialog rethrows this
               // exception but leaves the Window open and visible.
               if (!ex.IsCritical() && _window.IsVisible) {
                  CloseWindowImmediatately();
               }

               CloseScreen();

               throw;
            }

            if (_lastWindowHandlerException != null) {
               throw _lastWindowHandlerException;
            }

            return dialogLifecycle.ScreenResult ?? new DialogScreenResult(false);
         } finally {
            Disconnect();

            if (_screen != null) {
               _screen
                  .Children
                  .Remove(dialogLifecycle);
            }

            if (_window.Owner != null) {
               // Fix for http://stackoverflow.com/questions/3144004/wpf-app-loses-completely-focus-on-window-close
               _window.Owner.Activate();
            }
         }
      }

      private void Initialize() {
         _window.Closed += HandleWindowClosedToDisconnected;

         _screenOps = new ScreenLifecycleOperations(_aggregator, _screen);
         _screenOps.Activate();

         try {
            bool windowImplementsViewInterface = ViewFactory.TryInitializeView(_window, _screen);
            if (!windowImplementsViewInterface) {
               _window.Content = ViewFactory.CreateView(_screen);
            }
         } catch (Exception ex) {
            if (!ex.IsCritical()) {
               CloseScreen();
            }

            throw;
         }

         _screen.Children.Add(this);

         _aggregator.Publish(
            WindowService.InitializeWindowEvent,
            new InitializeWindowEventArgs(_screen, _window)
         );

         AttachHandlers();
      }

      /// <summary>
      ///   This method is called when the close is triggered by code (instead of the
      ///   user clicking on the 'X' for example).
      /// </summary>
      /// <exception cref="ScreenLifecycleException">
      ///   The dialog was shown modal and a lifecycle event handler (RequestClose, 
      ///   Deactivate or Close) of the screen has thrown an exception.
      /// </exception>
      private void HandleClose(bool requestClose) {
         bool shouldClose;

         if (_modal) {
            if (!requestClose) {
               _window.Closing -= HandleWindowClosing;
            }

            // Any exception is rethrown by 'ShowModal'.
            _window.Close();
         } else {
            // We reimplement the close process to propagate exceptions back to the
            // caller of 'HandleClose' because 'Window.Close' would rethrow the any
            // exception on the dispatcher.
            try {
               if (requestClose) {
                  shouldClose = _screenOps.RequestClose();
               } else {
                  shouldClose = true;
               }
            } catch (ScreenLifecycleException) {
               // The screen itself makes sure that is consistently closed in case of
               // an exception.
               CloseWindowImmediatately();
               throw;
            }

            if (shouldClose) {
               // First close the 'Window' than the 'Screen' because the 'Screen' may
               // still be accessed by the 'Window'.
               CloseWindowImmediatately();

               // Exceptions are propagated to the caller.
               CloseScreen();
            }
         }
      }

      private void CloseScreen() {
         var ops = new ScreenLifecycleOperations(_aggregator, _screen);
         ops.Deactivate();
         ops.Close();
      }

      /// <summary>
      ///   Directly closes the window without executing the <see cref="HandleWindowClosing"/> 
      ///   or <see cref="HandleWindowClosed"/>.
      /// </summary>
      private void CloseWindowImmediatately() {
         DetachHandlers();
         _window.Close();
      }

      private void Disconnect() {
         DetachHandlers();

         if (_screen != null) {
            _screen.Children.Remove(this);
         }

         _window.Closed -= HandleWindowClosedToDisconnected;
      }

      //
      //   W I N D O W   E V E N T   H A N D L I N G
      // 

      private void AttachHandlers() {
         _window.Closed += HandleWindowClosed;
         _window.Closing += HandleWindowClosing;

         _screen.Children.Add(_closeHandler);
      }

      private void DetachHandlers() {
         _window.Closed -= HandleWindowClosed;
         _window.Closing -= HandleWindowClosing;

         if (_screen != null) {
            _screen.Children.Remove(_closeHandler);
         }
      }

      /// <summary>
      ///  This handler is called if the close is trigger by the user (clicking the
      ///  'X' for example).
      /// </summary>
      private void HandleWindowClosing(object sender, CancelEventArgs e) {
         try {
            // If an exception is thrown:
            //  (1) the 'Window' continues to close. 
            //  (2) The screen itself makes sure it is consitently closed.
            //  (3) We just hope that no one else was attached a 'Closing' event 
            //      handler that sets 'Cancel' to 'true'.
            bool shouldClose = _screenOps.RequestClose();
            e.Cancel = !shouldClose;

            // The screen is closed by 'HandleWindowClosed'. We do not close the screen
            // now because (1) some code may still access the screen between now and 
            // 'Closed' and (2) an other 'Closing' event handler may set 'Cancel' to true.
         } catch (ScreenLifecycleException ex) {
            ProcessWindowEventHandlerException(ex);
         }
      }

      /// <remarks>
      ///   Only called if the window is closed by the user (clicking the X).
      /// </remarks>
      private void HandleWindowClosed(object sender, EventArgs e) {
         try {
            CloseScreen();
         } catch (ScreenLifecycleException ex) {
            ProcessWindowEventHandlerException(ex);
         }
      }

      /// <summary>
      ///   Always called to clean up and remove obsolete references.
      /// </summary>
      private void HandleWindowClosedToDisconnected(object sender, EventArgs e) {
         Disconnect();
      }

      /// <remarks>
      ///   <para>WPF behaves very strange when an exception is thrown in a 'Closed' or 
      ///      'Closing' event handler: the exception is swalled and the 'ShowDialog' call 
      ///      returns but the window still stays open and seems half disposed.</para>
      ///   <para>To work arround this we catch the exception ourself. If the window
      ///     was shown with 'ShowDialog' we save the exception and rethrow it after 
      ///     'ShowDialog' returns, so the caller can handle the exception. If the 
      ///     window was shown with 'Show' we rethrow the exception with the 'Dispatcher'
      ///     so that is treated as an unhandled application excpetion.</para>
      /// </remarks>
      private void ProcessWindowEventHandlerException(ScreenLifecycleException ex) {
         if (_modal) {
            _lastWindowHandlerException = ex;
         } else {
            Dispatcher.CurrentDispatcher.BeginInvoke(
               new Action(() => { throw ex; }),
               DispatcherPriority.Send
            );
         }
      }
   }
}
