namespace Inspiring.MvvmExample.Screens {
   using Inspiring.Mvvm.Screens;

   public sealed class ShellScreen : Screen, INeedsInitialization {
      public ScreenConductor WorkScreens { get; private set; }

      public void Initialize() {
         WorkScreens = Children.AddNew(ScreenFactory.For<ScreenConductor>());
      }
   }
}
