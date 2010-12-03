namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class DefaultPropertyBehaviorFactory : IBehaviorFactory {
      private BehaviorKey _behavior;

      public DefaultPropertyBehaviorFactory(BehaviorKey behaviorToCreate) {
         Contract.Requires<ArgumentNullException>(behaviorToCreate != null);

         _behavior = behaviorToCreate;
      }

      public IBehavior Create<T>() {
         if (_behavior == BehaviorKeys.InvalidDisplayValueCache) {
            return new AllowInvalidDisplayValuesBehavior();
         }

         if (_behavior == BehaviorKeys.DisplayValueAccessor) {
            return new DisplayValueAccessorBehavior<T>();
         }

         if (_behavior == BehaviorKeys.PropertyChangedTrigger) {
            return new PropertyChangedBehavior<T>();
         }

         if (_behavior == BehaviorKeys.TypeDescriptor) {
            return new PropertyDescriptorBehavior();
         }

         if (_behavior == BehaviorKeys.Validator) {
            return new ViewModelValidationBehavior();
         }

         if (_behavior == BehaviorKeys.PropertyValueCache) {
            return new RefreshableValueCahche<T>();
         }

         throw new NotSupportedException(
            ExceptionTexts.BehaviorNotSupportedByFactory.FormatWith(_behavior)
         );
      }
   }
}
