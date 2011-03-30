namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public class ViewModelBehaviorFactory : IBehaviorFactory {
      public static readonly ViewModelBehaviorFactory Instance = new ViewModelBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM>()
         where TVM : IViewModel {

         return new ViewModelBehaviorFactoryInvoker<TVM>();
      }

      public virtual IBehavior Create<TVM>(BehaviorKey key) where TVM : IViewModel {
         if (key == BehaviorKeys.Validator) {
            return new ViewModelValidationBehavior();
         }

         if (key == BehaviorKeys.ManualUpdateCoordinator) {
            return new LoadOrderBehavior();
         }

         if (key == BehaviorKeys.TypeDescriptor) {
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
