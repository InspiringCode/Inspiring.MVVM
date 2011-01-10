namespace Inspiring.Mvvm.ViewModels {

   public interface IHasSourceObject<TItemSource> : ICanInitializeFrom<TItemSource> {
      TItemSource Source { get; }
   }
}
