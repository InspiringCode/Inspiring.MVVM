namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public class PropertyBehaviorFactory : IBehaviorFactory {
      public static readonly PropertyBehaviorFactory Instance = new PropertyBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM, TValue>()
         where TVM : IViewModel {

         return new PropertyBehaviorFactoryInvoker<TVM, TValue>();
      }

      public virtual IBehavior Create<TVM, TValue>(BehaviorKey key) where TVM : IViewModel {
         if (key == BehaviorKeys.InvalidDisplayValueCache) {
            return new AllowInvalidDisplayValuesBehavior();
         }

         if (key == BehaviorKeys.DisplayValueAccessor) {
            return new DisplayValueAccessorBehavior<TValue>();
         }

         if (key == BehaviorKeys.PropertyChangedTrigger) {
            return new PropertyChangedBehavior<TValue>();
         }

         if (key == BehaviorKeys.TypeDescriptor) {
            return new PropertyDescriptorBehavior();
         }

         if (key == BehaviorKeys.PreValidationValueCache) {
            return new PreValidationValueCacheBehavior<TValue>();
         }

         if (key == BehaviorKeys.Validator) {
            return new PropertyValidationBehavior<TValue>();
         }

         if (key == BehaviorKeys.PropertyValueCache) {
            return new RefreshableValueCacheBehavior<TValue>();
         }

         if (key == BehaviorKeys.ValueCache) {
            return new ValueCacheBehavior<TValue>();
         }

         if (key == BehaviorKeys.CollectionInstanceCache) {
            return new ValueCacheBehavior<TValue>();
         }

         if (key == BehaviorKeys.ManualUpdateBehavior) {
            return new ManualUpdatePropertyBehavior();
         }

         if (key == BehaviorKeys.DescendantValidator) {
            return new DescendantValidationBehavior<TValue>();
         }

         if (key == BehaviorKeys.IsLoadedIndicator) {
            return new IsLoadedIndicatorBehavior<TValue>();
         }

         throw new NotSupportedException(
            ExceptionTexts.BehaviorNotSupportedByFactory.FormatWith(key)
         );
      }


      private class PropertyBehaviorFactoryInvoker<TVM, TValue> :
         BehaviorFactoryInvoker
         where TVM : IViewModel {

         public override IBehavior Invoke(IBehaviorFactory factory, BehaviorKey behaviorToCreate) {
            var typedFactory = CastFactory<PropertyBehaviorFactory>(factory, behaviorToCreate);
            return typedFactory.Create<TVM, TValue>(behaviorToCreate);
         }
      }

      IBehavior IBehaviorFactory.Create<T>() {
         throw new NotImplementedException();
      }
   }
}
