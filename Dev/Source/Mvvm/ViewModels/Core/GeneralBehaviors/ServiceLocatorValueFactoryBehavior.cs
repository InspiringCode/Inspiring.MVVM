namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Creates new instances of the view model type specified with <typeparamref 
   ///   name="TVM"/> using the <see cref="IBehaviorContext.ServiceLocator"/> of the
   ///   <see cref="IBehaviorContext"/>.
   /// </summary>
   internal sealed class ServiceLocatorValueFactoryBehavior<TVM> :
      Behavior,
      IViewModelFactoryBehavior<TVM>,
      IValueFactoryBehavior<TVM>
      where TVM : IViewModel { // TODO: Remove constraint, update documentation...

      public TVM CreateInstance(IBehaviorContext vm) {
         return vm.ServiceLocator.GetInstance<TVM>();
      }

      public TVM CreateValue(IBehaviorContext context) {
         return context.ServiceLocator.GetInstance<TVM>();
      }
   }
}
