namespace Inspiring.Mvvm.Views {
   using System;
   using System.Windows;
   using System.Windows.Media;
   using Inspiring.Mvvm.Screens;

   public class WindowService : IWindowService {
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

      public virtual void InitializeWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> withScreen
      ) where TScreen : IScreenBase {
         IScreenBase s = withScreen.Create(x => { });
         InitializeWindowCore(window, s, new WindowCloseHandler(s));
      }

      public virtual void InitializeDialogWindow<TScreen>(
         Window window,
         IScreenFactory<TScreen> withScreen
      ) where TScreen : IScreenBase {
         IScreenBase s = withScreen.Create(x => { });
         s.Children.Add(new DialogLifecycle());
         InitializeWindowCore(window, s, new DialogCloseHandler(s));
      }

      public virtual void ShowDialogWindow(IScreenBase screen, IScreenBase parent, string title) {
         Window dialogWindow = CreateDialogWindow();

         InitializeWindowCore(dialogWindow, screen, new DialogCloseHandler(screen));

         if (title != null) {
            dialogWindow.Title = title;
         }

         if (parent != null) {
            Window owner = GetAssociatedWindow(parent);
            dialogWindow.Owner = owner;
         }

         dialogWindow.ShowInTaskbar = false;
         dialogWindow.ShowDialog();
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

      private void InitializeWindowCore(
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

      private class WindowLifecycle : ScreenLifecycle {
         public Window AssociatedWindow { get; set; }
      }
   }
}
