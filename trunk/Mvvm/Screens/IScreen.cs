namespace Inspiring.Mvvm.Screens {
   public interface IScreenBase : IScreenLifecycle {
      ScreenLifecycleCollection<IScreenLifecycle> Children { get; }
   }
}
