namespace Inspiring.Mvvm.ViewModels {

   public interface IVMCollectionItem<TItemSource> : ICanInitializeFrom<TItemSource> {
      TItemSource Source { get; }
   }
}
