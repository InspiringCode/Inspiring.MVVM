namespace Inspiring.Mvvm.ViewModels.Core.GeneralBehaviors.ViewModelProperty {
   using System;

   internal sealed class ViewModelPropertyBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue> {



      public void Initialize(BehaviorInitializationContext context) {
         throw new NotImplementedException();
      }

      public TValue GetValue(IBehaviorContext context) {
         throw new NotImplementedException();
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         throw new NotImplementedException();
      }

   }
}
