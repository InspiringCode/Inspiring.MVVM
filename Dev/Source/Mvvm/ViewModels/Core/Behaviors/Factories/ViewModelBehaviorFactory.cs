namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public class ViewModelBehaviorFactory : IBehaviorFactoryProvider {
      public static readonly ViewModelBehaviorFactory Instance = new ViewModelBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM>()
         where TVM : IViewModel {

         return new ViewModelBehaviorFactoryInvoker<TVM>();
      }

      public virtual IBehavior Create<TVM>(BehaviorKey key) where TVM : IViewModel {
         if (key == ViewModelBehaviorKeys.ValidationExecutor) {
            return new ValidatorExecutorBehavior();
         }

         if (key == ViewModelBehaviorKeys.ViewModelValidationSource) {
            return new ViewModelValidationSourceBehavior();
         }

         if (key == PropertyBehaviorKeys.ManualUpdateCoordinator) {
            return new LoadOrderBehavior();
         }

         if (key == PropertyBehaviorKeys.PropertyDescriptorProvider) {
            return new TypeDescriptorBehavior();
         }

         if (key == ViewModelBehaviorKeys.UndoRoot) {
            return new UndoRootBehavior();
         }

         throw new NotSupportedException(
            ExceptionTexts.BehaviorNotSupportedByFactory.FormatWith(key)
         );
      }

      private class ViewModelBehaviorFactoryInvoker<TVM> :
         BehaviorFactoryInvoker
         where TVM : IViewModel {

         public override IBehavior Invoke(IBehaviorFactoryProvider factory, BehaviorKey behaviorToCreate) {
            var typedFactory = CastFactory<ViewModelBehaviorFactory>(factory, behaviorToCreate);
            return typedFactory.Create<TVM>(behaviorToCreate);
         }
      }
   }
}
