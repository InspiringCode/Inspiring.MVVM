namespace Inspiring.Mvvm.Views {
   using System;
   using System.Windows;
   using System.Windows.Media;
   using Inspiring.Mvvm.Screens;

   // TODO: Refactor the WindowService/DialogService abstraction!
   //       Suggestion: WindowService should be only responsible for creating
   //                   and managing 'Window' object and not for screen related
   //                   stuff.
   //                   Maybe introduce ScreenService (or rename DialogService)?
   // TODO: Make sure that the tests for 'DialogService' are written for WindowService
   //       as well.
   public class WindowService : IWindowService {
      private readonly ScreenService _screenService = new ScreenService();

      public Window CreateWindow<TScreen>(
         IScreenFactory<TScreen> initializeWithScreen
      ) where TScreen : IScreenBase {
         Window window = CreateWindow();
         InitializeWindow(window, initializeWithScreen);
         return window;
      }
      public Window CreateDialogWindow<TScreen>(
         IScreenFactory<TScreen> initializeWithScreen
      ) where TScreen : IScreenBase {
         Window window = CreateDialogWindow();
         InitializeDialogWindow(window, initializeWithScreen);
         return window;
      }
      public Window CreateShellWindow<TScreen>(
         IScreenFactory<TScreen> initializeWithScreen
      ) where TScreen : IScreenBase {
         Window window = CreateShellWindow();
         InitializeWindow(window, initializeWithScreen);
         return window;
      }

      public void InitializeWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> withScreen
      ) where TScreen : IScreenBase {
         TScreen s = _screenService.CreateAndActivateScreen(withScreen);

         InitializeWindowInternal(window, s, new WindowCloseHandler(s));
      }

      public void InitializeDialogWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> withScreen
      ) where TScreen : IScreenBase {
         TScreen s = _screenService.CreateAndActivateScreen(
            withScreen,
            initializationCallback: x => x.Children.Add(new DialogLifecycle())
         );

         InitializeWindowInternal(window, s, new DialogCloseHandler(s));
      }

      /// <summary>
      /// 
      /// </summary>
      /// <remarks>
      ///   Does not change the <paramref name="screen"/> if an exception is thrown in the
      ///   dialog.
      /// </remarks>
      public virtual void ShowDialogWindow(IScreenBase screen, IScreenBase parent, string title) {
         Window dialogWindow = CreateDialogWindow();

         // It is important, that we do these initializations FIRST, because 
         // 'InitializeWindowCore' may override these properties.
         dialogWindow.ShowInTaskbar = false;

         if (title != null) {
            dialogWindow.Title = title;
         }

         if (parent != null) {
            Window owner = GetAssociatedWindow(parent);
            dialogWindow.Owner = owner;
         }

         DialogCloseHandler closeHandler = new DialogCloseHandler(screen);

         // May throw an exception. The remaining code is correctly skipped in this
         // case.
         InitializeWindowInternal(dialogWindow, screen, closeHandler);

         // TODO: This code is at the wrong level of abstraction?!
         if (parent != null) {
            screen.Children.Expose<ScreenHierarchyLifecycle>().Opener = parent;
            parent.Children.Expose<ScreenHierarchyLifecycle>().OpenedScreens.Add(screen);
         }

         try {
            InvokeShowDialog(dialogWindow);

            // Only called if no exception is thrown
            _screenService.DeactivateAndCloseScreen(screen);
         } catch (Exception) {
            // If an exception is thrown in the UI code of the dialog window (e.g. in
            // a command handler or on a Dispatcher action), ShowDialog rethrows this
            // exception but leaves the Window open and visible.
            if (dialogWindow.IsVisible) {
               dialogWindow.Close();
            }

            throw;
         } finally {
            if (parent != null) {
               screen.Children.Expose<ScreenHierarchyLifecycle>().Opener = null;
               parent.Children.Expose<ScreenHierarchyLifecycle>().OpenedScreens.Remove(screen);
            }
         }
      }

      public Window GetAssociatedWindow(IScreenBase ofScreen) {
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

      // Für Unit Testing.
      internal virtual Nullable<bool> InvokeShowDialog(Window dialog) {
         return dialog.ShowDialog();
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

      protected virtual void InitializeWindowCore(Window window, IScreenBase screen) {
      }

      /// <summary>
      ///   Creates and/or initializes the appropriate view and initializes the <see 
      ///   cref="Window"/>. Does not change anything if the view initialization throws
      ///   an exception (e.g. the constructor of the <see cref="IView{T}"/> implementation).
      /// </summary>
      private void InitializeWindowInternal(
         Window window,
         IScreenBase forScreen,
         WindowCloseHandler closeHandler
      ) {
         // Create and/or set the model property of the view/window FIRST. If this
         // code block throws an exception, the remaining code is correctly skipped!
         bool windowImplementsViewInterface = ViewFactory.TryInitializeView(window, forScreen);
         if (!windowImplementsViewInterface) {
            window.Content = ViewFactory.CreateView(forScreen);
         }

         // Save the window for later
         var windowLifecycle = new WindowLifecycle { AssociatedWindow = window };
         forScreen.Children.Add(windowLifecycle);

         window.Closed += delegate {
            forScreen.Children.Remove(windowLifecycle);
         };

         closeHandler.AttachTo(window);

         InitializeWindowCore(window, forScreen);
      }

      private class WindowLifecycle : ScreenLifecycle {
         public Window AssociatedWindow { get; set; }
      }
   }
}