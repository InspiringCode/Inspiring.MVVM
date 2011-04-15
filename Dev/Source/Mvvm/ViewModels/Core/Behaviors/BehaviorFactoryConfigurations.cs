namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Common.Behaviors;

   internal static class BehaviorFactoryConfigurations {
      public static IBehaviorFactoryConfiguration ForSimpleProperty<TOwnerVM, TValue, TSourceObject>(
         bool propertyHasSource
      ) where TOwnerVM : IViewModel {
         return new SimplePropertyBehaviorFactoryConfiguration<TOwnerVM, TValue, TSourceObject>(propertyHasSource);
      }

      public static IBehaviorFactoryConfiguration ForChildProperty<TOwnerVM, TChildVM, TSourceObject>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel {

         return new ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject>();
      }

      public static IBehaviorFactoryConfiguration ForChildProperty<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel, IHasSourceObject<TChildSource> {

         return new ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject, TChildSource>();
      }

      public static IBehaviorFactoryConfiguration ForViewModel<TVM>()
         where TVM : IViewModel {

         return new ViewModelBehaviorFactoryConfiguration<TVM>();
      }

      private class SimplePropertyBehaviorFactoryConfiguration<TOwnerVM, TValue, TSourceObject> :
         BehaviorFactoryConfiguration<ISimplePropertyProvider>
         where TOwnerVM : IViewModel {

         private readonly bool _propertyHasSource;

         public SimplePropertyBehaviorFactoryConfiguration(bool propertyHasSource) {
            _propertyHasSource = propertyHasSource;
         }

         protected override IBehaviorFactory GetFactory(ISimplePropertyProvider factoryProvider) {
            return _propertyHasSource ?
               factoryProvider.GetFactoryForPropertyWithSource<TOwnerVM, TValue, TSourceObject>() :
               factoryProvider.GetFactory<TOwnerVM, TValue, TSourceObject>();
         }
      }

      private class ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject> :
         BehaviorFactoryConfiguration<IChildProvider>
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel {

         protected override IBehaviorFactory GetFactory(IChildProvider factoryProvider) {
            return factoryProvider.GetFactory<TOwnerVM, TChildVM, TSourceObject>();
         }
      }

      private class ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject, TChildSource> :
         BehaviorFactoryConfiguration<IChildProvider>
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel, IHasSourceObject<TChildSource> {

         protected override IBehaviorFactory GetFactory(IChildProvider factoryProvider) {
            return factoryProvider.GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>();
         }
      }

      private class ViewModelBehaviorFactoryConfiguration<TVM> :
         BehaviorFactoryConfiguration<IViewModelProvider>
         where TVM : IViewModel {

         protected override IBehaviorFactory GetFactory(IViewModelProvider factoryProvider) {
            return factoryProvider.GetFactory<TVM>();
         }
      }
   }
}
