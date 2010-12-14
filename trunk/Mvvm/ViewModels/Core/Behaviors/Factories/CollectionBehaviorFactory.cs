namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public class CollectionBehaviorFactory : IBehaviorFactory {
      public static readonly CollectionBehaviorFactory Instance = new CollectionBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM, TItemVM>()
         where TVM : IViewModel
         where TItemVM : IViewModel {

         return new CollectionBehaviorFactoryInvoker<TVM, TItemVM>();
      }

      public virtual IBehavior Create<TVM, TItemVM>(BehaviorKey key)
         where TVM : IViewModel
         where TItemVM : IViewModel {

         if (key == CollectionBehaviorKeys.ParentSetter) {
            return new ParentSetterCollectionBehavior<TItemVM>();
         }

         if (key == CollectionBehaviorKeys.ChangeNotifier) {
            return new ChangeNotifierCollectionBehavior<TItemVM>();
         }

         if (key == CollectionBehaviorKeys.ViewModelFactory) {
            return new ViewModelFactoryBehavior<TItemVM>();
         }

         throw new NotSupportedException(
            ExceptionTexts.BehaviorNotSupportedByFactory.FormatWith(key)
         );
      }

      private class CollectionBehaviorFactoryInvoker<TVM, TItemVM> :
         BehaviorFactoryInvoker
         where TVM : IViewModel
         where TItemVM : IViewModel {

         public override IBehavior Invoke(IBehaviorFactory factory, BehaviorKey behaviorToCreate) {
            var typedFactory = CastFactory<CollectionBehaviorFactory>(factory, behaviorToCreate);
            return typedFactory.Create<TVM, TItemVM>(behaviorToCreate);
         }
      }

      IBehavior IBehaviorFactory.Create<T>() {
         throw new NotImplementedException();
      }
   }
}
