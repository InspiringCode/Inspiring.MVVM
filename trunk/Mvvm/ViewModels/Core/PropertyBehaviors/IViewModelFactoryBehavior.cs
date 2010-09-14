namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IViewModelFactoryBehavior<TVM> : IBehavior where TVM : ViewModel {
      TVM CreateInstance(IBehaviorContext vm);
   }
}
