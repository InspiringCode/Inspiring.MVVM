namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common.Behaviors;

   internal sealed class PropertyBehaviorFactoryConfiguration<TOwner, TValue> :
      BehaviorFactoryConfiguration<PropertyBehaviorFactoryProvider>
      where TOwner : IViewModel {

      protected override IBehaviorFactory GetFactory(PropertyBehaviorFactoryProvider factoryProvider) {
         return factoryProvider.GetFactory<TOwner, TValue>();
      }
   }
}
