namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common.Behaviors;

   public class PropertyBehaviorFactoryProviderBase : IBehaviorFactoryProvider {
      public BehaviorFactory GetFactory<TOwnerVM, TValue>()
         where TOwnerVM : IViewModel {

         return new BehaviorFactory();
      }
   }

   public class PropertyBehaviorFactoryProvider : PropertyBehaviorFactoryProviderBase {
      public static readonly PropertyBehaviorFactoryProvider Default
         = new PropertyBehaviorFactoryProvider();

      public virtual new BehaviorFactory GetFactory<TOwnerVM, TValue>()
         where TOwnerVM : IViewModel {

         return base
            .GetFactory<TOwnerVM, TValue>();
      }
   }

   public class ViewModelPropertyBehaviorFactoryProvider : PropertyBehaviorFactoryProviderBase {
      public virtual new BehaviorFactory GetFactory<TOwnerVM, TValue>()
         where TOwnerVM : IViewModel
         where TValue : IViewModel {

         return base
            .GetFactory<TOwnerVM, TValue>();
      }
   }

   public class CollectionPropertyBehaviorFactoryProvider : PropertyBehaviorFactoryProviderBase {
      public virtual new BehaviorFactory GetFactory<TOwnerVM, TItemVM>()
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         return base
            .GetFactory<TOwnerVM, IVMCollection<TItemVM>>();
      }
   }
}
