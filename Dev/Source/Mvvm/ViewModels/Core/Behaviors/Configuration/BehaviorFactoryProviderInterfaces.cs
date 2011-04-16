namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public abstract class BehaviorFactoryProviderInterfaces {
      protected interface ISimplePropertyProvider : IBehaviorFactoryProvider {
         BehaviorFactory GetFactory<TOwnerVM, TValue, TSourceObject>()
            where TOwnerVM : IViewModel;

         BehaviorFactory GetFactoryForPropertyWithSource<TOwnerVM, TValue, TSourceObject>()
            where TOwnerVM : IViewModel;
      }

      protected interface IChildProvider : IBehaviorFactoryProvider {
         BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>()
            where TOwnerVM : IViewModel
            where TChildVM : IViewModel;

         BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
            where TOwnerVM : IViewModel
            where TChildVM : IViewModel, IHasSourceObject<TChildSource>;
      }

      protected interface IViewModelProvider : IBehaviorFactoryProvider {
         BehaviorFactory GetFactory<TVM>() where TVM : IViewModel;
      }
   }
}
