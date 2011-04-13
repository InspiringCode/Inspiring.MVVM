namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public class ViewModelPropertyProvider : PropertyProvider, IChildProvider {
      public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel {

         return GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>();
      }

      public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel, IHasSourceObject<TChildSource> {

         return GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>();
      }

      protected virtual new BehaviorFactory GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>()
         where TValue : IViewModel {

         return base
            .GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>()
            .RegisterBehavior<ViewModelInitializerBehavior<TValue>>(PropertyBehaviorKeys.ValueInitializer)
            .RegisterBehavior<LazyRefreshBehavior>(PropertyBehaviorKeys.LazyRefresh)
            .RegisterBehavior<ViewModelPropertyDescendantsValidatorBehavior<TValue>>(PropertyBehaviorKeys.DescendantsValidator)
            .RegisterBehavior<ServiceLocatorValueFactoryBehavior<TValue>>(PropertyBehaviorKeys.ValueFactory);
      }
   }
}
