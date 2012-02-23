namespace Inspiring.Mvvm.Screens {
   public interface IScreenBase : IScreenLifecycle {
      IScreenBase Parent { get; set; }
      ScreenLifecycleCollection<IScreenLifecycle> Children { get; }
   }
}
