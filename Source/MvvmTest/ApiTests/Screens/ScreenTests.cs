namespace Inspiring.MvvmTest.ApiTests.Screens {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;

   [TestClass]
   public class ScreenTests : ScreenTestBase {
      [TestMethod]
      public void RequestClose_StopsWhenIsCloseAllowedIsSetToFalse() {
         var parentScreen = CreateScreen<TestScreen>();
         var childScreen = CreateScreen<TestScreen>();

         parentScreen.Children.Attach(childScreen);

         parentScreen.RequestClose1IsCloseAllowed = false;

         Assert.IsFalse(GetOperationsFor(parentScreen).RequestClose());
         Assert.IsFalse(parentScreen.RequestClose2WasCalled);
         Assert.IsFalse(childScreen.RequestClose1WasCalled);
         Assert.IsFalse(childScreen.RequestClose2WasCalled);
      }

      private class TestScreen : DefaultTestScreen {
         public TestScreen(EventAggregator aggregator)
            : base(aggregator) {

            Lifecycle.RegisterHandler(
               ScreenEvents.RequestClose,
               args => {
                  RequestClose1WasCalled = true;
                  args.IsCloseAllowed = RequestClose1IsCloseAllowed;
               },
               ExecutionOrder.Default
            );

            Lifecycle.RegisterHandler(
               ScreenEvents.RequestClose,
               args => {
                  RequestClose2WasCalled = true;
                  args.IsCloseAllowed = RequestClose2IsCloseAllowed;
               },
               ExecutionOrder.AfterDefault
            );
         }

         public bool RequestClose1IsCloseAllowed { get; set; }
         public bool RequestClose2IsCloseAllowed { get; set; }

         public bool RequestClose1WasCalled { get; private set; }
         public bool RequestClose2WasCalled { get; private set; }
      }
   }
}