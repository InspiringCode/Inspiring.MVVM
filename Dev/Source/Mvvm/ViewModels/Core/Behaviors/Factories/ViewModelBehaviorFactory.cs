namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.ViewModels.Core.Validation.ViewModelBehaviors;

   public class ViewModelBehaviorFactory : IBehaviorFactory {
      public static readonly ViewModelBehaviorFactory Instance = new ViewModelBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM>()
         where TVM : IViewModel {

         return new ViewModelBehaviorFactoryInvoker<TVM>();
      }

      public IBehavior Create<TVM>(BehaviorKey key) where TVM : IViewModel {
         if (key == ViewModelBehaviorKeys.ValidationExecutor) {
            return new ValidatorExecutorBehavior();
         }

         if (key == PropertyBehaviorKeys.Validator) {
            return new ViewModelValidationBehavior();
         }

         if (key == PropertyBehaviorKeys.ManualUpdateCoordinator) {
            return new LoadOrderBehavior();
         }

         if (key == PropertyBehaviorKeys.TypeDescriptor) {
            return new TypeDescriptorBehavior();
         }

         throw new NotSupportedException(
            ExceptionTexts.BehaviorNotSupportedByFactory.FormatWith(key)
         );
      }

      private class ViewModelBehaviorFactoryInvoker<TVM> :
         BehaviorFactoryInvoker
         where TVM : IViewModel {

         public override IBehavior Invoke(IBehaviorFactory factory, BehaviorKey behaviorToCreate) {
            var typedFactory = CastFactory<ViewModelBehaviorFactory>(factory, behaviorToCreate);
            return typedFactory.Create<TVM>(behaviorToCreate);
         }
      }
   }
}
