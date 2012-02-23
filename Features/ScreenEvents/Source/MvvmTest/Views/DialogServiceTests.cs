namespace Inspiring.MvvmTest.Views {
   using System;
   using System.Linq;
   using System.Windows;
   using System.Windows.Threading;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Views;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DialogServiceTests : ScreenLifecycleTestBase {
      private WindowServiceStub WindowService { get; set; }
      private IDialogService DialogSerivce { get; set; }

      [TestInitialize]
      public void Setup() {
         WindowService = new WindowServiceStub();
         DialogSerivce = new DialogService(new EventAggregator(), WindowService);
      }

      [TestMethod]
      public void ShowDialogWithCertainScreenInsatnce_ScreenLifecycleDoesntContainDialogLifecycleAfterShowDialog() {
         WindowService.ShowRealWindow = false;

         var screen = new ScreenMock();
         DialogSerivce.ShowDialog(ScreenFactory.For(screen));
         Assert.IsFalse(screen.Children.OfType<DialogLifecycle>().Any());
      }

      [TestMethod]
      public void ShowDialog_WhenInitializeThrowsException_DoesNotShowDialogAndCallsClose() {
         var screen = new ScreenMock { ThrowOnInitialize = true };

         ShowDialogAndExpectException(screen);

         Assert.IsFalse(WindowService.WasShown);

         Assert.IsFalse(screen.WasActivated);
         Assert.IsFalse(screen.WasCloseRequested);
         Assert.IsFalse(screen.WasDeactivated);
         Assert.IsTrue(screen.WasClosed);
      }

      [TestMethod]
      public void ShowDialog_WhenActivateThrowsException_DoesNotShowDialog() {
         var screen = new ScreenMock { ThrowOnActivate = true };

         ShowDialogAndExpectException(screen);

         Assert.IsFalse(WindowService.WasShown);

         Assert.IsFalse(screen.WasCloseRequested);
         Assert.IsFalse(screen.WasDeactivated);
         Assert.IsTrue(screen.WasClosed);
      }

      [TestMethod]
      public void CloseDialog_WhenDeactivateThrowsException_ClosesDialog() {
         var screen = new ScreenMock { ThrowOnDeactivate = true };

         WindowService.WindowLoaded += (Window win) => {
            Assert.IsTrue(win.IsVisible);
            CloseDialogAndExpectException(screen);
         };

         ShowDialogAndExpectException(screen);

         Assert.IsTrue(screen.WasClosed);
         Assert.IsFalse(WindowService.LastWindow.IsVisible);
      }

      [TestMethod]
      public void CloseDialog_WhenOnCloseThrowsException_ClosesDialog() {
         var screen = new ScreenMock { ThrowOnClose = true };

         WindowService.WindowLoaded += (Window win) => {
            Assert.IsTrue(win.IsVisible);
            CloseDialogAndExpectException(screen);
         };

         ShowDialogAndExpectException(screen);

         Assert.IsFalse(WindowService.LastWindow.IsVisible);
      }

      [TestMethod]
      public void ShowDialog_WhenExceptionOccursInShownView_ClosesDialog() {
         var screen = new ScreenMock();

         WindowService.WindowLoaded += win => {
            win.Dispatcher.BeginInvoke(
               new Action(() => {
                  throw new ScreenMockException();
               }),
               DispatcherPriority.Background
            );
         };

         ShowDialogAndExpectException(screen);

         Assert.IsFalse(WindowService.LastWindow.IsVisible);
         Assert.IsFalse(screen.WasDeactivated);
         Assert.IsFalse(screen.WasClosed);
      }

      [TestMethod]
      public void ShowDialog_WhenViewInitializationThrowsException_DoesNotShowWindow() {
         WindowService.ThrowViewInitializationException = true;

         var screen = new ScreenMock();
         ShowDialogAndExpectException(screen);

         Assert.IsFalse(WindowService.WasShown);

         Assert.IsFalse(screen.WasDeactivated);
         Assert.IsFalse(screen.WasClosed);
      }

      [TestCleanup]
      public void CleanUp() {
         Dispatcher.CurrentDispatcher.InvokeShutdown();
      }

      private void ShowDialogAndExpectException(IScreenBase screen) {
         AssertHelper.Throws<ScreenMockException>(
            () => ShowDialog(screen),
            unwrapTargetInvocationException: true
         );
      }

      private void ShowDialog(IScreenBase screen) {
         DialogSerivce.ShowDialog(ScreenFactory.For(screen));
      }

      private void CloseDialogAndExpectException(IScreenBase screen) {
         AssertHelper.Throws<ScreenMockException>(() =>
            CloseDialog(screen)
         );
      }

      private void CloseDialog(IScreenBase screen) {
         screen.CloseDialog(new DialogScreenResult(true));
      }

      private class WindowServiceStub : WindowService {
         public WindowServiceStub() {
            ShowRealWindow = true;
         }

         public bool WasShown { get; private set; }
         public Window LastWindow { get; private set; }
         public event Action<Window> WindowLoaded;
         public bool ShowRealWindow { get; set; }
         public bool ThrowViewInitializationException { get; set; }

         public override Window CreateWindow(Window owner, string title, bool modal) {
            return new WindowView(ThrowViewInitializationException);
         }

         public override void ShowWindow(Window window, bool modal) {
            WasShown = true;
            LastWindow = window;

            window.Loaded += delegate {
               if (WindowLoaded != null) {
                  WindowLoaded(window);
               }
            };

            window.Opacity = 0;
            window.AllowsTransparency = true;
            window.WindowStyle = WindowStyle.None;
            window.ShowInTaskbar = false;

            if (ShowRealWindow) {
               base.ShowWindow(window, modal);
            }            
         }
      }

      private class WindowView : Window, IView<ScreenMock> {
         private readonly bool _throwModelSetterException;

         public WindowView(bool throwModelSetterException) {
            _throwModelSetterException = throwModelSetterException;
         }

         public ScreenMock Model {
            set {
               if (_throwModelSetterException) {
                  throw new ScreenMockException();
               }
            }
         }
      }
   }
}
