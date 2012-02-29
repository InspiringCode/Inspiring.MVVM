namespace Inspiring.MvvmExample.Screens {
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;

   public sealed class ShellScreen : ScreenBase {
      public ShellScreen(EventAggregator aggregator)
         : base(aggregator) {
         WorkScreens = Children.AddScreen(ScreenFactory.For<ScreenConductor>());
      }

      public ScreenConductor WorkScreens { get; private set; }
   }
}
