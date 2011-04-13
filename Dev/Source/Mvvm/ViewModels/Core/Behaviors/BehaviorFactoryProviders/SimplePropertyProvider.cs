namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public class SimplePropertyProvider : PropertyProvider, ISimplePropertyProvider {
      public virtual BehaviorFactory GetFactory<TOwnerVM, TValue, TSourceObject>()
         where TOwnerVM : IViewModel {
         return GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>();
      }

      public virtual BehaviorFactory GetFactoryForPropertyWithSource<TOwnerVM, TValue, TSourceObject>()
         where TOwnerVM : IViewModel {
         return GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>();
      }
   }
}
