namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.ViewModels.Core.Validation.PropertyBehaviors;

   public class ViewModelPropertyBehaviorFactory : IBehaviorFactory {
      public static readonly ViewModelPropertyBehaviorFactory Instance = new ViewModelPropertyBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM, TChildVM>()
         where TVM : IViewModel
         where TChildVM : IViewModel {

         return new ViewModelPropertyBehaviorFactoryInvoker<TVM, TChildVM>();
      }

      public virtual IBehavior Create<TVM, TChildVM>(BehaviorKey key)
         where TVM : IViewModel
         where TChildVM : IViewModel {

         if (key == PropertyBehaviorKeys.LazyRefresh) {
            return new LazyRefreshBehavior();
         }

         if (key == PropertyBehaviorKeys.ValueInitializer) {
            return new ValueInitializerBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.DisplayValueAccessor) {
            return new DisplayValueAccessorBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.TypeDescriptor) {
            return new PropertyDescriptorBehavior();
         }

         if (key == PropertyBehaviorKeys.ValueValidationSource) {
            return new ValueValidationSourceBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.ValueCache) {
            return new ValueCacheBehaviorOld<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.ValueCacheNew) {
            return new ValueCacheBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.ParentSetter) {
            return new ParentSetterBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.ViewModelFactory) {
            return new ServiceLocatorValueFactoryBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.DescendantsValidator) {
            return new ViewModelPropertyDescendantsValidatorBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.IsLoadedIndicator) {
            return new IsLoadedIndicatorBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.PropertyChangedTrigger) {
            return new PropertyChangedBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.UntypedValueAccessor) {
            return new UntypedPropertyAccessorBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.Undo) {
            return new UndoSetValueBehavior<TChildVM>();
         }

         throw new NotSupportedException(
            ExceptionTexts.BehaviorNotSupportedByFactory.FormatWith(key)
         );
      }


      private class ViewModelPropertyBehaviorFactoryInvoker<TVM, TChildVM> :
         BehaviorFactoryInvoker
         where TVM : IViewModel
         where TChildVM : IViewModel {

         public override IBehavior Invoke(IBehaviorFactory factory, BehaviorKey behaviorToCreate) {
            var typedFactory = CastFactory<ViewModelPropertyBehaviorFactory>(factory, behaviorToCreate);
            return typedFactory.Create<TVM, TChildVM>(behaviorToCreate);
         }
      }
   }
}
