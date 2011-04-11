namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IViewModelFactoryBehavior<TVM> : IBehavior where TVM : IViewModel {
      TVM CreateInstance(IBehaviorContext vm);
   }
}
