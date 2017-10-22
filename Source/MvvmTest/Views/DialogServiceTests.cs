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
      private EventAggregator Aggregator { get; set; }
      private WindowServiceStub WindowService { get; set; }
      private DialogService DialogSerivce { get; set; }

      [TestInitialize]
      public void Setup() {
         Aggregator = new EventAggregator();
         WindowService = new WindowServiceStub();
         DialogSerivce = new DialogService(Aggregator, WindowService);
      }

      [TestMethod]
      public void ShowDialogWithCertainScreenInstance_ScreenLifecycleDoesntContainDialogLifecycleAfterShowDialog() {
         WindowService.ShowRealWindow = false;

         var screen = new ScreenMock(Aggregator);
         DialogSerivce.ShowDialog(ScreenFactory.For(screen));
         Assert.IsFalse(screen.Children.OfType<DialogLifecycle>().Any());
      }

      [TestMethod]
      public void Close_WithoutRequestClose_DoesNotCallRequestClose() {
         WindowService.ShowRealWindow = true;

         var screen = new ScreenMock(Aggregator);

         WindowService.WindowLoaded += delegate {
            ScreenHelper.Close(screen, new DialogScreenResult(false), requestClose: false);
         };

         ShowDialog(screen);

         Assert.IsTrue(screen.WasClosed);
         Assert.IsFalse(screen.WasCloseRequested);
      }

      [TestMethod]
      public void Close_WithtRequestClose_CallsRequestClose() {
         WindowService.ShowRealWindow = true;

         var screen = new ScreenMock(Aggregator);

         WindowService.WindowLoaded += delegate {
            ScreenHelper.Close(screen, new DialogScreenResult(false), requestClose: true);
         };

         ShowDialog(screen);

         Assert.IsTrue(screen.WasClosed);
         Assert.IsTrue(screen.WasCloseRequested);
      }

      [TestMethod]
      public void ShowDialog_WhenInitializeThrowsException_DoesNotShowDialogAndCallsClose() {
         var screen = new ScreenMock(Aggregator) { ThrowOnInitialize = true };

         ShowDialogAndExpectException(screen);

         Assert.IsFalse(WindowService.WasShown);

         Assert.IsFalse(screen.WasActivated);
         Assert.IsFalse(screen.WasCloseRequested);
         Assert.IsFalse(screen.WasDeactivated);
         Assert.IsTrue(screen.WasClosed);
      }

      [TestMethod]
      public void ShowDialog_WhenActivateThrowsException_DoesNotShowDialog() {
         var screen = new ScreenMock(Aggregator) { ThrowOnActivate = true };

         ShowDialogAndExpectException(screen);

         Assert.IsFalse(WindowService.WasShown);

         Assert.IsFalse(screen.WasCloseRequested);
         Assert.IsTrue(screen.WasDeactivated);
         Assert.IsTrue(screen.WasClosed);
      }

      [TestMethod]
      public void CloseDialog_WhenDeactivateThrowsException_ClosesDialogAndOnlyShowDialogThrowsException() {
         var screen = new ScreenMock(Aggregator) { ThrowOnDeactivate = true };

         WindowService.WindowLoaded += (Window win) => {
            Assert.IsTrue(win.IsVisible);
            CloseDialog(screen);
         };

         ShowDialogAndExpectException(screen);

         Assert.IsTrue(screen.WasClosed);
         Assert.IsFalse(WindowService.LastWindow.IsVisible);
      }

      [TestMethod]
      public void CloseDialog_WhenOnCloseThrowsException_ClosesDialogAndOnlyShowDialogThrowsException() {
         var screen = new ScreenMock(Aggregator) { ThrowOnClose = true };

         WindowService.WindowLoaded += (Window win) => {
            Assert.IsTrue(win.IsVisible);
            CloseDialog(screen);
         };

         ShowDialogAndExpectException(screen);

         Assert.IsFalse(WindowService.LastWindow.IsVisible);
      }

      [TestMethod]
      public void ShowDialog_WhenExceptionOccursInShownView_ClosesDialog() {
         var screen = new ScreenMock(Aggregator);

         WindowService.WindowLoaded += win => {
            win.Dispatcher.BeginInvoke(
               new Action(() => {
                  throw new ScreenMockException();
               }),
               DispatcherPriority.Background
            );
         };

         AssertHelper.Throws<ScreenMockException>(() =>
            ShowDialog(screen)
         );

         Assert.IsFalse(WindowService.LastWindow.IsVisible);
         Assert.IsTrue(screen.WasDeactivated);
         Assert.IsTrue(screen.WasClosed);
      }

      [TestMethod]
      public void ShowDialog_WhenViewInitializationThrowsException_DoesNotShowWindow() {
         WindowService.ThrowViewInitializationException = true;

         var screen = new ScreenMock(Aggregator);

         AssertHelper.Throws<ScreenMockException>(() =>
            ShowDialog(screen)
         );

         Assert.IsFalse(WindowService.WasShown);

         Assert.IsTrue(screen.WasDeactivated);
         Assert.IsTrue(screen.WasClosed);
      }

      [TestMethod]
      public void Show_AttachesAppropriateChildren() {
         WindowService.ShowRealWindow = false;

         var screen = new ScreenMock(Aggregator);

         DialogSerivce.Show(
            new WindowView(false),
            ScreenFactory.For(screen),
            modal: false
         );

         Assert.IsTrue(screen.Children.OfType<WindowLifecycle>().Any());
         Assert.IsTrue(screen.Children.OfType<ScreenCloseHandler>().Any());
         Assert.IsFalse(screen.Children.OfType<DialogLifecycle>().Any());
      }

      [TestMethod]
      public void GetAssociatedWindow_ReturnsWindow() {
         WindowService.ShowRealWindow = false;

         WindowView window = new WindowView(false);
         ScreenMock screen = new ScreenMock(Aggregator);

         DialogSerivce.Show(
            window,
            ScreenFactory.For(screen),
            modal: false
         );

         Assert.AreEqual(window, DialogSerivce.GetAssociatedWindow(screen));
      }

      private void ShowDialogAndExpectException(IScreenBase screen) {
         AssertHelper.Throws<ScreenLifecycleException>(
            () => ShowDialog(screen),
            unwrapTargetInvocationException: true
         );
      }

      private void ShowDialog(IScreenBase screen) {
         DialogSerivce.ShowDialog(ScreenFactory.For(screen));
      }

      private void CloseDialogAndExpectException(IScreenBase screen) {
         AssertHelper.Throws<ScreenLifecycleException>(() =>
            CloseDialog(screen)
         );
      }

      private void CloseDialog(IScreenBase screen) {
         ScreenHelper.Close(screen, new DialogScreenResult(true));
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
