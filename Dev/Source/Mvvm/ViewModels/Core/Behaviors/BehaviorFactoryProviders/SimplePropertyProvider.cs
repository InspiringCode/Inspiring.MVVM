namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public class SimplePropertyProvider : ISimplePropertyProvider {
      public virtual BehaviorFactory GetFactory<TOwnerVM, TValue, TSourceObject>()
         where TOwnerVM : IViewModel {
         return GetCommonFactory<TOwnerVM, TValue, TSourceObject>();
      }

      public virtual BehaviorFactory GetFactoryForPropertyWithSource<TOwnerVM, TValue, TSourceObject>()
         where TOwnerVM : IViewModel {
         return GetCommonFactory<TOwnerVM, TValue, TSourceObject>();
      }

      protected virtual BehaviorFactory GetCommonFactory<TOwnerVM, TValue, TSourceObject>() {
         return new BehaviorFactory()
            .RegisterBehavior<DisplayValueAccessorBehavior<TValue>>(PropertyBehaviorKeys.DisplayValueAccessor);
      }
   }
}
