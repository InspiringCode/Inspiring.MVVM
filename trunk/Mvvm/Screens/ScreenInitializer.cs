namespace Inspiring.Mvvm.Screens {

   internal sealed class ScreenInitializer : IScreenInitializer {
      private IScreen _parent;

      public ScreenInitializer(IScreen parent) {
         _parent = parent;
      }

      public void Initialize(IScreen screen) {
         screen.Initialize();
         screen.Parent = _parent;
      }

      public void Initialize<TSubject>(IScreen<TSubject> screen, TSubject subject) {
         Initialize(screen);
         screen.Initialize(subject);
      }
   }
}
