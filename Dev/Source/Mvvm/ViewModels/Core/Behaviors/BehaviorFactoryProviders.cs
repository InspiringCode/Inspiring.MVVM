namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common.Behaviors;

   internal class PropertyBehaviorFactoryProvider : IBehaviorFactoryProvider {
      public virtual BehaviorFactory GetFactory<TOwnerVM, TValue>() {
         return new BehaviorFactory();
      }
   }
}
