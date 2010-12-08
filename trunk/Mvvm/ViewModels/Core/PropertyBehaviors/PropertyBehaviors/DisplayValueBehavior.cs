namespace Inspiring.Mvvm.ViewModels.Core.PropertyBehaviors.PropertyBehaviors {
   using System;

   internal sealed class DisplayValueBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IDisplayValueAccessorBehavior,
      IValidationStateProviderBehavior,
      IHandlePropertyChangedBehavior {

      public void Initialize(BehaviorInitializationContext context) {
         throw new NotImplementedException();
      }

      public object GetDisplayValue(IBehaviorContext vm) {
         throw new NotImplementedException();
      }

      public void SetDisplayValue(IBehaviorContext vm, object value) {
         throw new NotImplementedException();
      }

      public ValidationState GetValidationState(IBehaviorContext context) {
         throw new NotImplementedException();
      }

      public void HandlePropertyChanged(IBehaviorContext vm) {
         throw new NotImplementedException();
      }
   }
}
