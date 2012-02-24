namespace Inspiring.MvvmExample.Screens {
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Common;

   public sealed class ShellScreen : ScreenBase, INeedsInitialization {
      public ShellScreen(EventAggregator aggregator)
         : base(aggregator) {
      }

      public ScreenConductor WorkScreens { get; private set; }

      public void Initialize() {
         WorkScreens = Children.AddScreen(ScreenFactory.For<ScreenConductor>());
      }
   }
}
