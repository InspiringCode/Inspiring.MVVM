namespace Inspiring.MvvmTest.Views {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Views;
   using Inspiring.Mvvm.Common;

   [TestClass]
   public class ScreenConductorAdapterTests : ScreenTestBase {
      private ScreenConductor Conductor { get; set; }
      private ScreenConductorAdapter Adapter { get; set; }

      [TestInitialize]
      public void Setup() {
         Conductor = CreateScreen<ScreenConductor>();
         Adapter = new TestScreenConductorAdapter(Conductor);
      }

      [TestMethod]
      public void CloseView_CallsRequestClose() {
         bool isCloseAllowed = false;

         TestScreen screen = new TestScreen(Aggregator);
         screen.Lifecycle.RegisterHandler(
            ScreenEvents.RequestClose,
            args => args.IsCloseAllowed = isCloseAllowed
         );

         Conductor.OpenScreen(ScreenFactory.For(screen));

         object view = Adapter
            .Views
            .Cast<Object>()
            .Single();

         Assert.IsFalse(Adapter.CloseView(view));

         isCloseAllowed = true;

         Assert.IsTrue(Adapter.CloseView(view));
      }

      private class TestScreen : DefaultTestScreen {
         public TestScreen(EventAggregator aggregator)
            : base(aggregator) {
         }
      }

      private class TestScreenConductorAdapter : ScreenConductorAdapter {
         public TestScreenConductorAdapter(ScreenConductor screens)
            : base(screens) {
         }

         protected override object CreateView(IScreenBase forScreen) {
            return forScreen;
         }
      }
   }
}