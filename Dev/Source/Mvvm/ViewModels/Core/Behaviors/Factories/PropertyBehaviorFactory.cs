namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public class PropertyBehaviorFactory : IBehaviorFactory {
      public static readonly PropertyBehaviorFactory Instance = new PropertyBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM, TValue>()
         where TVM : IViewModel {

         return new PropertyBehaviorFactoryInvoker<TVM, TValue>();
      }

      public virtual IBehavior Create<TVM, TValue>(BehaviorKey key) where TVM : IViewModel {
         if (key == PropertyBehaviorKeys.InvalidDisplayValueCache) {
            return new AllowInvalidDisplayValuesBehavior();
         }

         if (key == PropertyBehaviorKeys.DisplayValueAccessor) {
            return new DisplayValueAccessorBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.PropertyChangedTrigger) {
            return new PropertyChangedBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.TypeDescriptor) {
            return new PropertyDescriptorBehavior();
         }

         if (key == PropertyBehaviorKeys.PreValidationValueCache) {
            return new PreValidationValueCacheBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.Validator) {
            return new PropertyValidationBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.PropertyValueCache) {
            return new RefreshableValueCacheBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.ValueCache) {
            return new ValueCacheBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.CollectionInstanceCache) {
            return new ValueCacheBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.ManualUpdateBehavior) {
            return new ManualUpdatePropertyBehavior();
         }

         if (key == PropertyBehaviorKeys.DescendantValidator) {
            return new DescendantValidationBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.IsLoadedIndicator) {
            return new IsLoadedIndicatorBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.UntypedValueAccessor) {
            return new UntypedPropertyAccessorBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.Undo) {
            return new UndoSetValueBehavior<TValue>();
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
   }
}
