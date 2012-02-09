namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public abstract class ScreenLifecycleTestBase {
      protected class ScreenMock : ScreenBase, INeedsInitialization {
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

         public bool RequestCloseResult { get; set; }

         public ScreenMock() {
            RequestCloseResult = true;
         }

         public void Initialize() {
            Assert.IsFalse(WasInitialized, "Initialize was called twice.");
            WasInitialized = true;
            if (ThrowOnInitialize) {
               throw new ScreenMockException();
            }
         }

         protected override void OnActivate() {
            _activateInvocations++;

            Assert.AreEqual(
               _activateInvocations - 1,
               _deactivateInvocations,
               "Activate was called twice."
            );

            base.OnActivate();

            if (ThrowOnActivate) {
               throw new ScreenMockException();
            }
         }

         protected override void OnDeactivate() {
            _deactivateInvocations++;

            Assert.AreEqual(
               _activateInvocations,
               _deactivateInvocations,
               "Deactivate was called twice."
            );

            base.OnDeactivate();

            if (ThrowOnDeactivate) {
               throw new ScreenMockException();
            }
         }

         protected override bool OnRequestClose() {
            Assert.IsFalse(WasCloseRequested, "RequestClose was called twice.");
            WasCloseRequested = true;
            return base.OnRequestClose() && RequestCloseResult;
         }

         protected override void OnClose() {
            Assert.IsFalse(WasClosed, "Close was called twice.");
            base.OnClose();
            WasClosed = true;
            if (ThrowOnClose) {
               throw new ScreenMockException();
            }
         }
      }

      protected class ScreenMockException : Exception { }
   }
}
