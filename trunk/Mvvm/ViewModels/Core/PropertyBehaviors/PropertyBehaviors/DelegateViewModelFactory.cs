namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class DelegateViewModelFactory<TSourceObject, TValue> :
      Behavior,
      IViewModelFactoryBehavior<TValue>
      where TValue : IViewModel {

      private Func<TSourceObject, TValue> _customFactory;

      public DelegateViewModelFactory(Func<TSourceObject, TValue> customFactory) {
         Contract.Requires(customFactory != null);
         _customFactory = customFactory;
      }

      public TValue CreateInstance(IBehaviorContext vm) {
         return vm.ServiceLocator.GetInstance<TValue>();
      }
   }
}
