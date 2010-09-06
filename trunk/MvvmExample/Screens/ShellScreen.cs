namespace Inspiring.MvvmExample.Screens {
   using Inspiring.Mvvm.Screens;

   public sealed class ShellScreen : Screen {
      public ScreenConductor WorkScreens { get; private set; }

      protected override void OnInitialize() {
         base.OnInitialize();
         WorkScreens = Children.AddNew(ScreenFactory.For<ScreenConductor>());
      }
   }
}
