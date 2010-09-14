namespace Inspiring.Mvvm.ViewModels.Core {
   internal sealed class PropagateVMContextBehavior<TVM> : Behavior, IViewModelFactoryBehavior<TVM> where TVM : ViewModel {
      public TVM CreateInstance(IBehaviorContext vm) {
         TVM viewModel = GetNextBehavior<IViewModelFactoryBehavior<TVM>>().CreateInstance(vm);
         viewModel.VMContext = vm.VMContext;
         return viewModel;
      }
   }
}
