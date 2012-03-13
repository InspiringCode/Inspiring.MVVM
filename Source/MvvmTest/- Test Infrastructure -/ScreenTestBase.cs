namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public abstract class ScreenTestBase {
      protected EventAggregator Aggregator { get; private set; }

      [TestInitialize]
      public void BaseSetup() {
         Aggregator = new EventAggregator();
      }

      public TScreen CreateScreen<TScreen>() where TScreen : ScreenBase {
         TScreen screen = (TScreen)Activator
            .CreateInstance(typeof(TScreen), Aggregator);

         GetOperationsFor(screen).Initialize();

         return screen;
      }

      protected ScreenLifecycleOperations GetOperationsFor(IScreenBase screen) {
         return new ScreenLifecycleOperations(Aggregator, screen);
      }
   }
}
