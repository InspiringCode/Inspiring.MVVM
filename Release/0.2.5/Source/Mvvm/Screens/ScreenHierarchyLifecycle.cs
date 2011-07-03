namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;

   public class ScreenHierarchyLifecycle : ScreenLifecycle {
      public ScreenHierarchyLifecycle() {
         OpenedScreens = new List<IScreenBase>();
      }

      public IScreenBase Opener { get; set; }

      public ICollection<IScreenBase> OpenedScreens { get; private set; }
   }
}
