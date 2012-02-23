namespace Inspiring.MvvmTest {
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;

   public class DefaultTestScreen : ScreenBase {
      public DefaultTestScreen()
         : this(new EventAggregator()) {
      }

      public DefaultTestScreen(EventAggregator aggregator)
         : base(aggregator) {
      }
   }
}
