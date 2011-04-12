namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Common.Behaviors;

   internal static class BehvaiorFactoryConfigurations {
      public static IBehaviorFactoryConfiguration GetForProperty<TOwnerVM, TValue, TSourceObject>()
         where TOwnerVM : IViewModel {
         return GetForPropertyWithSource<TOwnerVM, TValue, TSourceObject, TValue>();
      }

      public static IBehaviorFactoryConfiguration GetForPropertyWithSource<TOwnerVM, TValue, TSourceObject, TValueSource>()
         where TOwnerVM : IViewModel {
         return new SimplePropertyBehaviorFactoryConfiguration<TOwnerVM, TValue, TSourceObject, TValueSource>();
      }

      public static IBehaviorFactoryConfiguration GetForChildProperty<TOwnerVM, TChildVM, TSourceObject>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel {
         return new ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject>();
      }

      public static IBehaviorFactoryConfiguration GetForChildPropertyWithSource<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel, IHasSourceObject<TChildSource> {
         return new ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject, TChildSource>();
      }

      public static IBehaviorFactoryConfiguration GetForCollection<TOwnerVM, TChildVM, TSourceObject>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel {
         return new ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject>();
      }

      public static IBehaviorFactoryConfiguration GetForCollectionWithSource<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel, IHasSourceObject<TChildSource> {
         return new ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject, TChildSource>();
      }




      private class SimplePropertyBehaviorFactoryConfiguration<TOwnerVM, TValue, TSourceObject, TValueSource> :
         BehaviorFactoryConfiguration<ISimplePropertyBehaviorFactory>
         where TOwnerVM : IViewModel {

         protected override IBehaviorFactory GetFactory(ISimplePropertyBehaviorFactory factoryProvider) {
            return factoryProvider.GetFactory<TOwnerVM, TValue, TSourceObject, TValueSource>();
         }
      }

      private class ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject> :
         BehaviorFactoryConfiguration<IChildBehaviorFactoryProvider>
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel {

         protected override IBehaviorFactory GetFactory(IChildBehaviorFactoryProvider factoryProvider) {
            return factoryProvider.GetFactory<TOwnerVM, TChildVM, TSourceObject>();
         }
      }

      private class ChildBehaviorFactoryConfiguration<TOwnerVM, TChildVM, TSourceObject, TChildSource> :
         BehaviorFactoryConfiguration<IChildBehaviorFactoryProvider>
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel, IHasSourceObject<TChildSource> {

         protected IBehaviorFactory GetFactory(IChildWithSourceBehaviorFactoryProvider factoryProvider) {
            return factoryProvider.GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>();
         }

         protected override IBehaviorFactory GetFactory(IChildBehaviorFactoryProvider factoryProvider) {
            throw new NotImplementedException();
         }
      }

      private class ViewModelBehaviorFactoryConfiguration<TVM> :
         BehaviorFactoryConfiguration<IViewModelBehaviorFactoryProvider>
         where TVM : IViewModel {

         protected override IBehaviorFactory GetFactory(IViewModelBehaviorFactoryProvider factoryProvider) {
            return factoryProvider.GetFactory<TVM>();
         }
      }
   }








   internal sealed class PropertyBehaviorFactoryConfiguration<TOwner, TValue> :
      BehaviorFactoryConfiguration<PropertyBehaviorFactoryProvider>
      where TOwner : IViewModel {

      protected override IBehaviorFactory GetFactory(PropertyBehaviorFactoryProvider factoryProvider) {
         //return factoryProvider.GetFactory<TOwner, TValue>();
         throw new NotImplementedException();
      }
   }

   internal sealed class ViewModelPropertyBehaviorFactoryConfiguration<TOwnerVM, TValue> :
      BehaviorFactoryConfiguration<ViewModelPropertyBehaviorFactoryProvider>
      where TOwnerVM : IViewModel
      where TValue : IViewModel {

      protected override IBehaviorFactory GetFactory(ViewModelPropertyBehaviorFactoryProvider factoryProvider) {
         //return factoryProvider.GetFactory<TOwnerVM, TValue>();
         throw new NotImplementedException();
      }
   }

   internal sealed class CollectionPropertyBehaviorFactoryConfiguration<TOwnerVM, TItemVM> :
      BehaviorFactoryConfiguration<CollectionPropertyBehaviorFactoryProvider>
      where TOwnerVM : IViewModel
      where TItemVM : IViewModel {

      protected override IBehaviorFactory GetFactory(CollectionPropertyBehaviorFactoryProvider factoryProvider) {
         //return factoryProvider.GetFactory<TOwnerVM, TItemVM>();
         throw new NotImplementedException();
      }
   }

   internal sealed class ViewModelBehaviorFactoryConfiguration<TVM> :
      BehaviorFactoryConfiguration<ViewModelBehaviorFactoryProvider>
      where TVM : IViewModel {

      protected override IBehaviorFactory GetFactory(ViewModelBehaviorFactoryProvider factoryProvider) {
         //return factoryProvider.GetFactory<TVM>();
         throw new NotImplementedException();
      }
   }

   internal sealed class CommandBehaviorFactoryConfiguration<TOwnerVM, TSourceObject> :
      BehaviorFactoryConfiguration<CommandBehaviorFactoryProvider>
      where TOwnerVM : IViewModel {

      protected override IBehaviorFactory GetFactory(CommandBehaviorFactoryProvider factoryProvider) {
         //return factoryProvider.GetFactory<TOwnerVM, TSourceObject>();
         throw new NotImplementedException();
      }
   }
}
