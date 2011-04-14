namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public class PropertyBehaviorFactory : IBehaviorFactoryProvider {
      public static readonly PropertyBehaviorFactory Instance = new PropertyBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM, TValue>()
         where TVM : IViewModel {

         return new PropertyBehaviorFactoryInvoker<TVM, TValue>();
      }

      public virtual IBehavior Create<TVM, TValue>(BehaviorKey key) where TVM : IViewModel {
         if (key == PropertyBehaviorKeys.LazyRefresh) {
            return new LazyRefreshBehavior();
         }

         if (key == PropertyBehaviorKeys.DisplayValueAccessor) {
            return new DisplayValueAccessorBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.ChangeNotifier) {
            return new PropertyChangedNotifierBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.PropertyDescriptorProvider) {
            return new PropertyDescriptorProviderBehavior();
         }

         if (key == PropertyBehaviorKeys.ValueValidationSource) {
            return new ValueValidationSourceBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.PropertyValueCache) {
            return new RefreshableValueCacheBehavior<TValue>();
         }

         if (key == PropertyBehaviorKeys.ValueCache) {
            return new ValueCacheBehaviorOld<TValue>();
         }

         if (key == PropertyBehaviorKeys.CollectionInstanceCache) {
            return new ValueCacheBehaviorOld<TValue>();
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

         public override IBehavior Invoke(IBehaviorFactoryProvider factory, BehaviorKey behaviorToCreate) {
            var typedFactory = CastFactory<PropertyBehaviorFactory>(factory, behaviorToCreate);
            return typedFactory.Create<TVM, TValue>(behaviorToCreate);
         }
      }
   }
}
