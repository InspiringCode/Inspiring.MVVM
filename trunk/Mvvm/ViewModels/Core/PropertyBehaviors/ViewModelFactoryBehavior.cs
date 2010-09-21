﻿namespace Inspiring.Mvvm.ViewModels.Core {
   internal sealed class ViewModelFactoryBehavior<TVM> : Behavior, IViewModelFactoryBehavior<TVM> where TVM : ViewModel {
      public TVM CreateInstance(IBehaviorContext vm) {
         return vm.ServiceLocator.GetInstance<TVM>();
      }
   }
}
