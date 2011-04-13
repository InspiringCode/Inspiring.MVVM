namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public class ViewModelProvider : IViewModelProvider {
      public virtual BehaviorFactory GetFactory<TVM>() where TVM : IViewModel {
         return new BehaviorFactory();
      }
   }
}
