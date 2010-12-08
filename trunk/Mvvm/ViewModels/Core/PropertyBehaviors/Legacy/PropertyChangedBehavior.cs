namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public sealed class PropertyChangedBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue> {

      private VMPropertyBase<TValue> _property;

      public TValue GetValue(IBehaviorContext vm, ValueStage stage) {
         return GetNextBehavior<IValueAccessorBehavior<TValue>>().GetValue(vm, stage);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         TValue oldValue = GetValue(context, ValueStage.PostValidation);
         GetNextBehavior<IValueAccessorBehavior<TValue>>().SetValue(context, value);

         if (!Object.Equals(value, oldValue)) {
            var args = new ChangeArgs(ChangeType.PropertyChanged, context.VM, _property);
            context.NotifyChange(args);
         }
      }

      public void Initialize(BehaviorInitializationContext context) {
         _property = (VMPropertyBase<TValue>)context.Property;
         this.InitializeNext(context);
      }
   }
}
