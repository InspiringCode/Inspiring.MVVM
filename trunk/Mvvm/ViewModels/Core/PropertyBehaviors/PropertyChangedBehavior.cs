namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class PropertyChangedBehavior<TValue> : Behavior, IAccessPropertyBehavior<TValue> {
      private VMProperty<TValue> _property;

      public PropertyChangedBehavior(VMProperty<TValue> property) {
         Contract.Requires(property != null);
         _property = property;
      }

      public TValue GetValue(IBehaviorContext vm) {
         return GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         TValue oldValue = GetValue(vm);
         GetNextBehavior<IAccessPropertyBehavior<TValue>>().SetValue(vm, value);

         if (!Object.Equals(value, oldValue)) {
            vm.RaisePropertyChanged(_property);
         }
      }
   }
}
