namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ParentSetterBehavior<TValue> :
      Behavior,
      IValueAccessorBehavior<TValue>
      where TValue : IViewModel {

      public TValue GetValue(IBehaviorContext context, ValueStage stage = ValueStage.PreValidation) {
         return this.GetValueNext<TValue>(context, stage);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         var previous = this.GetValueNext<TValue>(context, ValueStage.PreValidation);

         if (!Object.ReferenceEquals(previous, value)) {
            if (previous != null) {
               previous.Kernel.Parent = null;
            }
            if (value != null) {
               value.Kernel.Parent = context.VM;
            }
         }

         this.SetValueNext(context, value);
      }
   }
}
