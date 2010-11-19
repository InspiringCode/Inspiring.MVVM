namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public sealed class PropertyChangedBehavior<TValue> : Behavior, IAccessPropertyBehavior<TValue> {
      private VMPropertyBase<TValue> _property;

      public TValue GetValue(IBehaviorContext vm) {
         return GetNextBehavior<IAccessPropertyBehavior<TValue>>().GetValue(vm);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         TValue oldValue = GetValue(context);
         GetNextBehavior<IAccessPropertyBehavior<TValue>>().SetValue(context, value);

         if (!Object.Equals(value, oldValue)) {
            var args = new ChangeArgs(ChangeType.PropertyChanged, context.VM);
            context.NotifyChange(args);
         }
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _property = (VMPropertyBase<TValue>)context.Property;
      }
   }
}
