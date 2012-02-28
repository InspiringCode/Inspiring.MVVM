namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public abstract class ScreenLifecycleTestBase {
      protected class ScreenMock : DefaultTestScreen {
         private int _activateInvocations = 0;
         private int _deactivateInvocations = 0;

         public bool WasInitialized { get; private set; }
         public bool WasActivated { get { return _activateInvocations > 0; } }
         public bool WasDeactivated { get { return _deactivateInvocations > 0; } }
         public bool WasClosed { get; private set; }
         public bool WasCloseRequested { get; private set; }

         public bool ThrowOnInitialize { get; set; }
         public bool ThrowOnActivate { get; set; }
         public bool ThrowOnDeactivate { get; set; }
         public bool ThrowOnClose { get; set; }
         public bool ThrowOnRequestClose { get; set; }

         public bool RequestCloseResult { get; set; }

         public ScreenMock(EventAggregator aggregator)
            : base(aggregator) {
            RequestCloseResult = true;

            Lifecycle.RegisterHandler(ScreenEvents.Initialize(), Initialize);
            Lifecycle.RegisterHandler(ScreenEvents.Activate, OnActivate);
            Lifecycle.RegisterHandler(ScreenEvents.Deactivate, OnDeactivate);
            Lifecycle.RegisterHandler(ScreenEvents.RequestClose, OnRequestClose);
            Lifecycle.RegisterHandler(ScreenEvents.Close, OnClose);
         }

         public void Initialize(InitializeEventArgs args) {
            Assert.IsFalse(WasInitialized, "Initialize was called twice.");
            WasInitialized = true;
            if (ThrowOnInitialize) {
               throw new ScreenMockException();
            }
         }

         protected void OnActivate(ScreenEventArgs args) {
            _activateInvocations++;

            Assert.AreEqual(
               _activateInvocations - 1,
               _deactivateInvocations,
               "Activate was called twice."
            );

            if (ThrowOnActivate) {
               throw new ScreenMockException();
            }
         }

         protected void OnDeactivate(ScreenEventArgs args) {
            _deactivateInvocations++;

            Assert.AreEqual(
               _activateInvocations,
               _deactivateInvocations,
               "Deactivate was called twice."
            );

            if (ThrowOnDeactivate) {
               throw new ScreenMockException();
            }
         }

         protected void OnRequestClose(RequestCloseEventArgs args) {
            Assert.IsFalse(WasCloseRequested, "RequestClose was called twice.");
            WasCloseRequested = true;

            if (ThrowOnRequestClose) {
               throw new ScreenMockException();
            }

            args.IsCloseAllowed = RequestCloseResult;
         }

         protected void OnClose(ScreenEventArgs args) {
            Assert.IsFalse(WasClosed, "Close was called twice.");
            WasClosed = true;
            if (ThrowOnClose) {
               throw new ScreenMockException();
            }
         }
      }

      protected class ScreenMockException : Exception { }
   }
}
