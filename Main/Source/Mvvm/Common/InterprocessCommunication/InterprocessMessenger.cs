namespace Inspiring.Mvvm.Common {
   using System;
   using System.Windows;
   using System.Windows.Interop;
   using System.Windows.Threading;
   using Inspiring.Mvvm.Common.Core;

   public class InterprocessMessenger : WindowMessageInterprocessMessenger {
      private Window _hiddenWindow = null;
      private HwndSourceHook _hookDelegate = null;
      private bool _isListening = false;

      public InterprocessMessenger(string sharedIdentifier = null)
         : base(sharedIdentifier) {

         _hookDelegate = HandleWindowHook;
      }

      public override bool IsListening {
         get { return _isListening; }
      }

      protected override void StartListeningCore() {
         _hiddenWindow = new Window();

         // Set the title of the Window so it can be identified as a valid receiver later.
         _hiddenWindow.Title = SharedIdentifier;

         IntPtr hwnd = new WindowInteropHelper(_hiddenWindow).EnsureHandle();
         HwndSource source = HwndSource.FromHwnd(hwnd);

         source.AddHook(_hookDelegate);

         _isListening = true;
      }

      protected override void StopListeningCore() {
         if (IsListening) {
            // It is especially important to dispatch call if Dispose is called
            // from the finalizer!
            Dispatcher d = _hiddenWindow.Dispatcher;
            d.Invoke(
               new Action(ReleaseWindow),
               DispatcherPriority.Send
            );

            _isListening = false;
         }
      }

      private void ReleaseWindow() {
         _hiddenWindow.Close();
         _hiddenWindow = null;
      }

      protected override bool IsOwnWindow(IntPtr windowHandle) {
         if (!IsListening) {
            return false;
         }

         IntPtr ownHandle = new WindowInteropHelper(_hiddenWindow).Handle;
         return windowHandle == ownHandle;
      }

      private IntPtr HandleWindowHook(
         IntPtr hwnd,
         int msg,
         IntPtr wParam,
         IntPtr lParam,
         ref bool handled
      ) {
         return WindowMessageHook(msg, wParam, lParam, ref handled);
      }
   }
}
