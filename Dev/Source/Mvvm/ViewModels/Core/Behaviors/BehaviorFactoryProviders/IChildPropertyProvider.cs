namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public interface IChildProvider : IBehaviorFactoryProvider {
      BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel;

      BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel, IHasSourceObject<TChildSource>;
   }
}
