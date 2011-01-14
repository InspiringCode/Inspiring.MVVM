namespace Inspiring.Mvvm.Screens {
   public interface IScreen : IScreenLifecycle {
      ScreenLifecycleCollection<IScreenLifecycle> Children { get; }
   }
}
