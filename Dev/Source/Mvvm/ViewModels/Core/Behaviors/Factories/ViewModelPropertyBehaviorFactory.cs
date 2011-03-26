namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ViewModelPropertyBehaviorFactory : IBehaviorFactory {
      public static readonly ViewModelPropertyBehaviorFactory Instance = new ViewModelPropertyBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM, TChildVM>()
         where TVM : IViewModel
         where TChildVM : IViewModel {

         return new ViewModelPropertyBehaviorFactoryInvoker<TVM, TChildVM>();
      }

      public IBehavior Create<TVM, TChildVM>(BehaviorKey key)
         where TVM : IViewModel
         where TChildVM : IViewModel {

         if (key == PropertyBehaviorKeys.DisplayValueAccessor) {
            return new DisplayValueAccessorBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.TypeDescriptor) {
            return new PropertyDescriptorBehavior();
         }

         if (key == PropertyBehaviorKeys.PreValidationValueCache) {
            return new PreValidationValueCacheBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.Validator) {
            return new PropertyValidationBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.ValueCache) {
            return new ValueCacheBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.ParentInitializer) {
            return new ParentSetterBehavior<TChildVM>(
               setParentOnGetValue: true,
               setParentOnSetValue: false
            );
         }

         if (key == PropertyBehaviorKeys.ParentSetter) {
            return new ParentSetterBehavior<TChildVM>(
               setParentOnGetValue: false,
               setParentOnSetValue: true
            );
         }

         if (key == PropertyBehaviorKeys.ViewModelFactory) {
            return new ViewModelFactoryBehavior<TChildVM>();
         }

         if (key == PropertyBehaviorKeys.DescendantValidator) {
            return new DescendantValidationBehavior<TChildVM>();
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
