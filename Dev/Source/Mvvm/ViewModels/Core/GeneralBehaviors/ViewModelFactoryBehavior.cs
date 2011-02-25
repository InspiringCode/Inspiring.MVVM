namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Creates new instances of the view model type specified with <typeparamref 
   ///   name="TVM"/> using the <see cref="IBehaviorContext.ServiceLocator"/> of the
   ///   <see cref="IBehaviorContext"/>.
   /// </summary>
   internal sealed class ViewModelFactoryBehavior<TVM> :
      Behavior,
      IViewModelFactoryBehavior<TVM>
      where TVM : IViewModel {

      public TVM CreateInstance(IBehaviorContext vm) {
         return vm.ServiceLocator.GetInstance<TVM>();
      }
   }
}
