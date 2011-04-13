namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public interface IViewModelProvider : IBehaviorFactoryProvider {
      BehaviorFactory GetFactory<TVM>() where TVM : IViewModel;
   }
}
