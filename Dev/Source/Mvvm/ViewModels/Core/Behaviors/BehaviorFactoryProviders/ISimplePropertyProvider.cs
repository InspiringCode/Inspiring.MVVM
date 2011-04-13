namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public interface ISimplePropertyProvider : IBehaviorFactoryProvider {
      BehaviorFactory GetFactory<TOwnerVM, TValue, TSourceObject>()
         where TOwnerVM : IViewModel;

      BehaviorFactory GetFactoryForPropertyWithSource<TOwnerVM, TValue, TSourceObject>()
         where TOwnerVM : IViewModel;
   }
}
