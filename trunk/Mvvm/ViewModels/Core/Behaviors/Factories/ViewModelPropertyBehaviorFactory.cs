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

         if (key == BehaviorKeys.DisplayValueAccessor) {
            return new DisplayValueAccessorBehavior<TChildVM>();
         }

         if (key == BehaviorKeys.TypeDescriptor) {
            return new PropertyDescriptorBehavior();
         }

         if (key == BehaviorKeys.PreValidationValueCache) {
            return new PreValidationValueCacheBehavior<TChildVM>();
         }

         if (key == BehaviorKeys.Validator) {
            return new PropertyValidationBehavior<TChildVM>();
         }

         if (key == BehaviorKeys.ValueCache) {
            return new ValueCacheBehavior<TChildVM>();
         }

         if (key == BehaviorKeys.ParentInitializer) {
            return new ParentSetterBehavior<TChildVM>(
               setParentOnGetValue: true,
               setParentOnSetValue: false
            );
         }

         if (key == BehaviorKeys.ParentSetter) {
            return new ParentSetterBehavior<TChildVM>(
               setParentOnGetValue: false,
               setParentOnSetValue: true
            );
         }

         if (key == BehaviorKeys.ViewModelFactory) {
            return new ViewModelFactoryBehavior<TChildVM>();
         }

         if (key == BehaviorKeys.DescendantValidator) {
            return new DescendantValidationBehavior<TChildVM>();
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

      IBehavior IBehaviorFactory.Create<T>() {
         throw new NotImplementedException();
      }
   }
}
