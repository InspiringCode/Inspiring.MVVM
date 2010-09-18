namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public sealed class PropertyChangedBehavior<TValue> : Behavior, IAccessPropertyBehavior<TValue> {
      private VMPropertyBase<TValue> _property;

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

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _property = (VMPropertyBase<TValue>)context.Property;
      }
   }
}
