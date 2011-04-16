namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Creates new instances of the type specified with <typeparamref 
   ///   name="TVM"/> using the <see cref="IBehaviorContext.ServiceLocator"/> of the
   ///   <see cref="IBehaviorContext"/>.
   /// </summary>
   internal sealed class ServiceLocatorValueFactoryBehavior<TVM> :
      Behavior,
      IValueFactoryBehavior<TVM> {

      public TVM CreateValue(IBehaviorContext context) {
         return context.ServiceLocator.GetInstance<TVM>();
      }
   }
}
