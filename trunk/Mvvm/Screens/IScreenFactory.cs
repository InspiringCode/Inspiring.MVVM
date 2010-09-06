namespace Inspiring.Mvvm.Screens {
   public interface IScreenFactory<TScreen> where TScreen : IScreen {
      TScreen Create(IScreenInitializer initializer);
   }

   public interface IScreenInitializer {
      void Initialize(IScreen screen);
      void Initialize<TSubject>(IScreen<TSubject> screen, TSubject subject);
   }
}
